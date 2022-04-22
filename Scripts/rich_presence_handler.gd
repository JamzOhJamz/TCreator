extends Node

var activity = Discord.Activity.new()

func _ready():
	activity.set_type(Discord.ActivityType.Playing)
	
	activity.set_details("Idling")

	var assets = activity.get_assets()
	assets.set_large_image("icon2")
	assets.set_large_text("TCreator")
	
	var timestamps = activity.get_timestamps()
	timestamps.set_start(OS.get_unix_time())

	_update_activity()

func _update_details(text):
	activity.set_details("Editing " + text)
	
	_update_activity()

func _update_activity():
	var result = yield(Discord.activity_manager.update_activity(activity), "result").result
	if result != Discord.Result.Ok:
		push_error(String(result))
