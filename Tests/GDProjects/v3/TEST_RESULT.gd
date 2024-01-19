extends Node

var file_name = ""

func _ready():
	var args = OS.get_cmdline_args()
	print("Args: ", args)
	
	for i in args.size():
		if args[i] == "--test_result_file":
			if args.size() > i+1:
				file_name = args[i+1]
				continue
	
	if file_name != "":
		var f = File.new()
		f.open(file_name, File.WRITE)
		f.store_string("SUCCESS")
		print("SUCCESS stored in " + file_name)
		f.close()
		get_tree().quit(0)
		return
	print("FAILED. No file is specified.")
	get_tree().quit(1)
	return
