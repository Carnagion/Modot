## Represents a modular component loaded at runtime, with its own scripts, resource packs, and data.
class_name Mod
extends RefCounted

## Initializes a new [Mod] using the [code]metadata[/code].
func _init(metadata):
	self._meta = metadata
	self._load_resources()
	self._load_data()
	self._load_scripts()

var _meta

var _data = {}

var _scripts = []

## The metadata of the [Mod], such as its ID, name, load order, etc.
var meta:
	get:
		return self._meta

## The JSON data of the [Mod], combined into a single JSON dictionary with the file names as keys and their parsed contents as values.
var data:
	get:
		return self._data

## The scripts of the [Mod].
var scripts:
	get:
		return self._scripts

func _load_resources():
	var resources_path = self.meta.directory.path_join("resources")
	var directory = Directory.new()
	if not directory.dir_exists(resources_path):
		return
	directory.open(resources_path)
	for unloaded_resource in DirectoryExtensions.get_files_recursive_ending(directory, ["pck"]).filter(func(resource_path): return not ProjectSettings.load_resource_pack(resource_path)):
		Errors._mod_load_error(self.meta.directory, "Could not load resource pack at %s" % unloaded_resource)

func _load_data():
	var data_path = self.meta.directory.path_join("data")
	var directory = Directory.new()
	if not directory.dir_exists(data_path):
		return
	directory.open(data_path)
	var file = File.new()
	for json_path in DirectoryExtensions.get_files_recursive_ending(directory, ["json"]):
		file.open(json_path, File.READ)
		var json = JSON.parse_string(file.get_as_text())
		file.close()
		if json == null:
			Errors._mod_load_error(self.meta.directory, "Could not parse JSON at %s" % json_path)
			continue
		self._data[json_path] = json

func _load_scripts():
	var scripts_path = self.meta.directory.path_join("scripts")
	self._scripts.append_array(self._load_code(scripts_path))

func _load_code(directory_path):
	var directory = Directory.new()
	if not directory.dir_exists(directory_path):
		return []
	directory.open(directory_path)
	var file = File.new()
	return DirectoryExtensions.get_files_recursive_ending(directory, ["gd"]).map(func(file_path):
			file.open(file_path, File.READ)
			var code = file.get_as_text()
			file.close()
			var script = GDScript.new()
			script.source_code = code
			return script)

func _to_string():
	return "{ meta: %s, data: %s, scripts: %s }" % [self.meta, self.data, self.scripts]

## Represents the metadata of a [Mod], such as its unique ID, name, author, load order, etc.
class Metadata extends RefCounted:
	
	var _directory
	
	var _id
	
	var _name
	
	var _author
	
	var _dependencies
	
	var _before
	
	var _after
	
	var _incompatible
	
	## The directory where the [Metadata] was loaded from.
	var directory:
		get:
			return self._directory
	
	## The unique ID of the [Mod].
	var id:
		get:
			return self._id
	
	## THe name of the [Mod].
	var name:
		get:
			return self._name
	
	## The individual or group that created the [Mod].
	var author:
		get:
			return self._author
	
	## The unique IDs of all other [Mod]s that the [Mod] depends on.
	var dependencies:
		get:
			return self._dependencies
	
	## The unique IDs of all other [Mod]s that should be loaded before the [Mod].
	var before:
		get:
			return self._before
	
	## The unique IDs of all other [Mod]s that should be loaded after the [Mod].
	var after:
		get:
			return self._after
	
	## The unique IDs of all other [Mod]s that are incompatible with the [Mod].
	var incompatible:
		get:
			return self._incompatible
	
	static func _load(directory_path):
		# Locate metadata file
		var metadata_file_path = directory_path.path_join("mod.json")
		var file = File.new()
		if not file.file_exists(metadata_file_path):
			Errors._mod_load_error(directory_path, "Mod metadata file does not exist")
			return null
		# Retrieve metadata file contents
		file.open(metadata_file_path, File.READ)
		var json = JSON.parse_string(file.get_as_text())
		file.close()
		if not json is Dictionary:
			Errors._mod_load_error(directory_path, "Mod metadata is invalid")
			return null
		var meta = Mod.Metadata.new()
		meta._directory = directory_path
		return meta if meta._try_deserialize(json) and meta._is_valid() else null
	
	func _try_deserialize(json):
		# Retrieve compulsory metadata
		var id = json.get("id")
		var name = json.get("name")
		var author = json.get("author")
		if not (id is String and name is String and author is String):
			Errors._mod_load_error(self.directory, "Mod metadata contains invalid ID, name, or author")
			return false
		self._id = id
		self._name = name
		self._author = author
		# Retrieve optional metadata
		var dependencies = json.get("dependencies", [])
		if dependencies is Array:
			self._dependencies = dependencies
		else:
			Errors._mod_load_error(self.directory, "Mod metadata contains invalid dependencies")
			return false
		var before = json.get("before", [])
		if before is Array:
			self._before = before
		else:
			Errors._mod_load_error(self.directory, "Mod metadata contains invalid load before list")
			return false
		var after = json.get("after", [])
		if after is Array:
			self._after = after
		else:
			Errors._mod_load_error(self.directory, "Mod metadata contains invalid load after list")
			return false
		var incompatible = json.get("incompatible", [])
		if incompatible is Array:
			self._incompatible = incompatible
		else:
			Errors._mod_load_error(self.directory, "Mod metadata contains invalid incompatibilities")
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
		Errors._mod_load_error(self.directory, "Mod metadata contains invalid load order or invalid dependencies")
		return false
	
	func _to_string():
		return "{ directory: %s, id: %s, name: %s, author: %s, dependencies: %s, before: %s, after: %s, incompatible: %s }" % [self.directory, self.id, self.name, self.author, self.dependencies, self.before, self.after, self.incompatible]
