extends Panel

var tModCreator

func _ready():
	tModCreator = get_node("/root/tModCreator")

func _on_NewBlueprintOption_mouse_entered():
	get("custom_styles/panel").set_bg_color(Color("1c1c24"))

func _on_NewBlueprintOption_mouse_exited():
	get("custom_styles/panel").set_bg_color(Color("16161d"))

func _on_NewBlueprintOption_gui_input(event):
	if (event is InputEventMouseButton && event.pressed && event.button_index == 1):
		tModCreator.get_node("CanvasLayer/NewBlueprintPopup").hide()
		tModCreator.get_node("CanvasLayer/Control").hide()
		tModCreator.get_node("CanvasLayer/Sidebar").show()
		tModCreator.set_navbar_text("Untitled Item")
		tModCreator.create_start_node_in_workspace()
		tModCreator.inWorkspace = true
