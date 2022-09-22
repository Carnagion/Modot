class_name Mod
extends RefCounted

func _init(metadata):
	self._meta = metadata
	self._load_resources()
	self._load_data()

var _meta

var _data = {}

var meta:
	get:
		return self._meta

var data:
	get:
		return self._data

func _load_resources():
	var resources_path = self.meta.directory.path_join("resources")
	var directory = Directory.new()
	directory.open(resources_path)
	for unloaded_resource in DirectoryExtensions.get_files_recursive_ending(directory, ["pck"]).filter(func(resource_path): return not ProjectSettings.load_resource_pack(resource_path)):
		Errors.mod_load_error(self.meta.directory, "Could not load resource pack at %s" % unloaded_resource)

func _load_data():
	var data_path = self.meta.directory.path_join("data")
	var directory = Directory.new()
	directory.open(data_path)
	var file = File.new()
	for json_path in DirectoryExtensions.get_files_recursive_ending(directory, ["json"]):
		file.open(json_path, File.READ)
		var json = JSON.parse_string(file.get_as_text())
		file.close()
		if json == null:
			Errors.mod_load_error(self.meta.directory, "Could not parse JSON at %s" % json_path)
			continue
		self._data[json_path] = json

class Metadata extends RefCounted:
	
	var _directory
	
	var _id
	
	var _name
	
	var _author
	
	var _dependencies
	
	var _before
	
	var _after
	
	var _incompatible
	
	var directory:
		get:
			return self._directory
	
	var id:
		get:
			return self._id
	
	var name:
		get:
			return self._name
	
	var author:
		get:
			return self._author
	
	var dependencies:
		get:
			return self._dependencies
	
	var before:
		get:
			return self._before
	
	var after:
		get:
			return self._after
	
	var incompatible:
		get:
			return self._incompatible
	
	static func _load(directory_path):
		var meta = Mod.Metadata.new()
		meta._directory = directory_path
		# Locate metadata file
		var metadata_file_path = directory_path.path_join("mod.json")
		var file = File.new()
		if not file.file_exists(metadata_file_path):
			Errors.mod_load_error(directory_path, "Mod metadata file does not exist")
			return null
		# Retrieve metadata file contents
		file.open(metadata_file_path, File.READ)
		var json = JSON.parse_string(file.get_as_text())
		file.close()
		if not json is Dictionary:
			Errors.mod_load_error(directory_path, "Mod metadata is invalid")
			return null
		return meta if meta._try_deserialize(json) and meta._is_valid() else null
		
	
	func _try_deserialize(json):
		# Retrieve compulsory metadata
		var id = json.get("id")
		var name = json.get("name")
		var author = json.get("author")
		if not (id is String and name is String and author is String):
			Errors.mod_load_error(self.directory, "Mod metadata contains invalid ID, name, or author")
			return false
		self._id = id
		self._name = name
		self._author = author
		# Retrieve optional metadata
		var dependencies = json.get("dependencies", [])
		if dependencies is Array:
			self._dependencies = dependencies
		else:
			Errors.mod_load_error(self.directory, "Mod metadata contains invalid dependencies")
			return false
		var before = json.get("before", [])
		if before is Array:
			self._before = before
		else:
			Errors.mod_load_error(self.directory, "Mod metadata contains invalid load before list")
			return false
		var after = json.get("after", [])
		if after is Array:
			self._after = after
		else:
			Errors.mod_load_error(self.directory, "Mod metadata contains invalid load after list")
			return false
		var incompatible = json.get("incompatible", [])
		if incompatible is Array:
			self._incompatible = incompatible
		else:
			Errors.mod_load_error(self.directory, "Mod metadata contains invalid incompatibilities")
			return false
		return true
	
	func _is_valid():
		# Check that the incompatible, load before, and load after lists don't have anything common or contain the mod's own ID
		var duplicates = {}
		var valid_load_order = ([self.id] + self.before + self.after + self.incompatible).filter(func(id):
				if id in duplicates:
					return true
				duplicates[id] = true
				return false).is_empty()
		# Check that the dependency and incompatible lists don't have anything in common
		var valid_dependencies = self.dependencies.filter(func(id): return id in incompatible).is_empty()
		if valid_load_order and valid_dependencies:
			return true
		Errors.mod_load_error(self.directory, "Mod metadata contains invalid load order or invalid dependencies")
		return false
