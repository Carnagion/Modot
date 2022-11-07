class_name StringExtensions

static func replace_once(string, substring, replacement):
	var index = string.findn(substring)
	match index:
		-1:
			return string
		_:
			var before = string.substr(0, index)
			var after = string.substr(index + substring.length())
			return before + replacement + after
