class_name ArrayExtensions

static func topological_sort(array, dependencies, cyclic):
	var sorted = []
	var states = {}
	var all_valid = array.all(func(element): return ArrayExtensions._visit_dependencies(element, dependencies, cyclic, sorted, states))
	return sorted if all_valid else []

static func _visit_dependencies(element, dependencies, cyclic, sorted, states):
	if not element in states:
		states[element] = false
	match states[element]:
		true:
			return true
		false:
			states[element] = null
			var dependencies_valid = dependencies.call(element).all(func(dependency): return ArrayExtensions._visit_dependencies(dependency, dependencies, cyclic, sorted, states))
			if not dependencies_valid:
				return false
			states[element] = true
			sorted.append(element)
			return true
		null:
			cyclic.call(element)
			return false
