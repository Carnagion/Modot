## Provides methods and properties for loading [Mod]s at runtime, obtaining all loaded [Mod]s, and finding a loaded [Mod] by its ID.
extends Node

var _loaded_mods = {}

## All the [Mod]s that have been loaded at runtime.
var loaded_mods:
	get:
		return self._loaded_mods

## Loads a [Mod] from [code]mod_directory_path[/code] and runs all [code]_init()[/code] functions in its scripts if [code]execute_scripts[/code] is true.
func load_mod(mod_directory_path, execute_scripts = true):
	var metadata = Mod.Metadata._load(mod_directory_path)
	if not metadata:
		return null
	var mod = Mod.new(metadata)
	self._loaded_mods[mod.meta.id] = mod
	if execute_scripts:
		self._startup_mod(mod)
	return mod

## Loads [Mod]s from [code]mod_directory_paths[/code] and runs all [code]_init()[/code] functions in their scripts if [code]execute_scripts[/code] is true.
func load_mods(mod_directory_paths, execute_scripts = true):
	var mods = []
	for metadata in self._sort_mod_metadata(self._filter_mod_metadata(self._load_mod_metadata(mod_directory_paths))):
		var mod = Mod.new(metadata)
		mods.append(mod)
		self._loaded_mods[metadata.id] = mod
	if execute_scripts:
		for mod in mods:
			self._startup_mod(mod)
	return mods

func _startup_mod(mod):
	for script in mod.scripts:
		script.reload()
		script.new()

func _load_mod_metadata(mod_directory_paths):
	var loaded_metadata = {}
	for metadata in mod_directory_paths.map(func(mod_directory_path): return Mod.Metadata._load(mod_directory_path)).filter(func(metadata): return metadata != null):
		# Fail if the metadata is incompatible with any of the loaded metadata (and vice-versa), or if the ID already exists
		var incompatible_metadata = metadata.incompatible.map(func(id): return loaded_metadata[id]).filter(func(loaded): return loaded != null) + loaded_metadata.values().filter(func(loaded): return metadata in loaded.incompatible)
		if not incompatible_metadata.is_empty():
			Errors._mod_load_error(metadata.directory, "Mod is incompatible with other loaded mods")
			continue
		elif metadata.id in loaded_metadata:
			Errors._mod_load_error(metadata.directory, "Mod has duplicate ID")
			continue
		loaded_metadata[metadata.id] = metadata
	return loaded_metadata

func _filter_mod_metadata(loaded_metadata):
	# If the dependencies of any metadata have not been loaded, remove that metadata and try again
	var invalid_metadata = loaded_metadata.values().filter(func(metadata): return metadata.dependencies.any(func(dependency): return not dependency in loaded_metadata))
	for metadata in invalid_metadata:
		Errors._mod_load_error(metadata.directory, "Not all dependencies are loaded")
		loaded_metadata.erase(metadata.id)
		return self._filter_mod_metadata(loaded_metadata)
	return loaded_metadata

func _sort_mod_metadata(filtered_metadata):
	if filtered_metadata.is_empty():
		return []
	# Create a graph of each metadata ID and the IDs of those that need to be loaded after it
	var dependency_graph = {}
	for metadata in filtered_metadata.values():
		if not metadata.id in dependency_graph:
			dependency_graph[metadata.id] = []
		for after in metadata.after:
			dependency_graph[metadata.id].append(after)
		for before in metadata.before:
			if not before in dependency_graph:
				dependency_graph[before] = []
				dependency_graph[before].append(metadata.id)
	# Topologically sort the dependency graph, removing cyclic dependencies if any
	var sorted_metadata = ArrayExtensions.topological_sort(dependency_graph.keys(), func(id): return dependency_graph.get(id, []), func(cyclic):
			Errors._mod_load_error(filtered_metadata[cyclic].directory, "Mod has cyclic dependencies with other mods")
			filtered_metadata.erase(cyclic))
	# If there is no valid topological sorting (cyclic dependencies detected), remove the cyclic metadata and try again
	if sorted_metadata.is_empty():
		return self._sort_mod_metadata(self._filter_mod_metadata(filtered_metadata))
	return sorted_metadata.map(func(id): return filtered_metadata.get(id)).filter(func(metadata): return metadata != null)
