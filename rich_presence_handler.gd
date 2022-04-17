extends Node

func _ready():
	var activity = Discord.Activity.new()
	activity.set_type(Discord.ActivityType.Playing)
	#activity.set_state("Reached: 2-2")
	activity.set_details("Editing Untitled-1")

	var assets = activity.get_assets()
	assets.set_large_image("icon2")
	assets.set_large_text("TCreator")
	
	var timestamps = activity.get_timestamps()
	timestamps.set_start(OS.get_unix_time())

	var result = yield(Discord.activity_manager.update_activity(activity), "result").result
	if result != Discord.Result.Ok:
		push_error(String(result))
