extends Node


func _ready():
	var args = OS.get_cmdline_user_args()
	var file_name = ""
	print("Args: ", args)
	
	for i in args.size():
		if args[i] == "--test_result_file":
			if args.size() > i+1:
				file_name = args[i+1]
				continue
	
	if file_name != "":
		var f = FileAccess.open(file_name, FileAccess.WRITE)
		f.store_string("SUCCESS")
		print("SUCCESS stored in " + file_name)
		get_tree().quit(0)
		return
	print("FAILED. No file is specified.")
	get_tree().quit(1)
	return
