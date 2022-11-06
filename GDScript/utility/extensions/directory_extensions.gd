## Contains utility methods for [Directory].
class_name DirectoryExtensions

## Copies all files from the directory at [code]from[/code] to the directory at [code]to[/code], recursively if specified.
static func copy_contents(directory, from, to, recursive = false):
	directory.open(from)
	# Create destination directory if it doesn't already exist
	directory.make_dir_recursive(to)
	var copied = []
	# Copy all files inside the source directory non-recursively
	for from_file in directory.get_files():
		from_file = from.path_join(from_file)
		var to_file = StringExtensions.replace_once(from_file, from, to)
		directory.copy(from_file, to_file)
		copied.append(to_file)
	if not recursive:
		return copied
	# Copy all files recursively
	for from_sub_directory in DirectoryExtensions.get_directories_recursive(directory):
		var to_sub_directory = StringExtensions.replace_once(from_sub_directory, from, to)
		directory.make_dir_recursive(to_sub_directory)
		var inner_directory = Directory.new()
		inner_directory.open(from_sub_directory)
		for from_file in inner_directory.get_files():
			from_file = from_sub_directory.path_join(from_file)
			var to_file = StringExtensions.replace_once(from_file, from, to)
			directory.copy(from_file, to_file)
			copied.append(to_file)
	return copied

## Returns the complete paths of all subdirectories inside [code]directory[/code], searching recursively.
static func get_directories_recursive(directory):
	var directories = []
	for subdir_path in directory.get_directories():
		var path_full = directory.get_current_dir().path_join(subdir_path)
		directories.append(path_full)
		var subdir = Directory.new()
		subdir.open(path_full)
		directories.append_array(DirectoryExtensions.get_directories_recursive(subdir))
	return directories

## Returns the complete paths of all files inside [code]directory[/code], searching recursively.
static func get_files_recursive(directory):
	var files = []
	for file_path in directory.get_files():
		files.append(directory.get_current_dir().path_join(file_path))
	for subdir_path in DirectoryExtensions.get_directories_recursive(directory):
		var subdir = Directory.new()
		subdir.open(directory.get_current_dir().path_join(subdir_path))
		files.append_array(subdir.get_files())
	return files

## Returns the complete file paths of all files inside [code]directory[/code] whose extensions match any of [code]extensions[/code], searching recursively.
static func get_files_recursive_ending(directory, extensions):
	return DirectoryExtensions.get_files_recursive(directory).filter(func(file_path): return extensions.any(func(extension): return file_path.ends_with(".%s" % extension)))
