class_name Errors

static func _mod_load_error(directory_path, message):
	push_error("Error loading mod at %s: %s" % [directory_path, message])
