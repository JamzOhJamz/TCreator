extends Node2D

var inWorkspace = false
var draggingComponent = false
var focusedOnControl = false
var nodeConnectionLineDrawing = false
var connectToInNub = null

func _ready():
	OS.set_window_maximized(true)
	pass

func _process(delta):
	if $CanvasLayer/Control.get_focus_owner() != null:
		focusedOnControl = true
	else:
		focusedOnControl = false

func _input(event):
	if event is InputEventMouseButton:
		if event.is_pressed():
			if event.button_index == BUTTON_LEFT or event.button_index == BUTTON_RIGHT:
				var focus_owner = $CanvasLayer/Control.get_focus_owner()
				if focus_owner != null:
					var rect = Rect2(focus_owner.rect_global_position, focus_owner.rect_size)
					if not rect.has_point(get_global_mouse_position()):
						if focus_owner is LineEdit:
							focus_owner.deselect()
						focus_owner.release_focus()

func _on_NewProjectBtn_gui_input(event):
	if (event is InputEventMouseButton && event.pressed && event.button_index == 1):
		print("Clicked")

func _on_NewProjectBtn_mouse_entered():
	$CanvasLayer/Control/VBoxContainer/NewProjectBtn.set("custom_colors/font_color", Color("ff8197"))

func _on_NewProjectBtn_mouse_exited():
	$CanvasLayer/Control/VBoxContainer/NewProjectBtn.set("custom_colors/font_color", Color("ec3b5a"))

func _on_NewBlueprintBtn_gui_input(event):
	if (event is InputEventMouseButton && event.pressed && event.button_index == 1):
		if not $CanvasLayer/NewBlueprintPopup.visible:
			$CanvasLayer/NewBlueprintPopup.show()
		else:
			$CanvasLayer/NewBlueprintPopup.hide()

func _on_NewBlueprintBtn_mouse_entered():
	$CanvasLayer/Control/VBoxContainer/NewBlueprintBtn.set("custom_colors/font_color", Color("ff8197"))

func _on_NewBlueprintBtn_mouse_exited():
	$CanvasLayer/Control/VBoxContainer/NewBlueprintBtn.set("custom_colors/font_color", Color("ec3b5a"))

func set_navbar_text(text):
	$CanvasLayer/Navbar/TitleText.set_text(text + " - TCreator Alpha")

func create_start_node_in_workspace():
	var start_node = load("res://Blueprints/StartNode.tscn").instance()
	start_node.rect_position.x = 0 - start_node.rect_size.x / 2
	start_node.rect_position.y = 0 - start_node.rect_size.y / 2
	$Workspace.add_child(start_node)

func show_workspace_position(workspace_position):
	$CanvasLayer/WorkspacePosition.modulate = Color(1, 1, 1, 1)
	var roundedX = round_to_dec(workspace_position.x, 1)
	var roundedY = round_to_dec(workspace_position.y, 1)
	$CanvasLayer/WorkspacePosition.text = "[" + String(roundedX) + ", " + String(roundedY) + "]"
	var tween = Tween.new()
	add_child(tween)
	tween.interpolate_property($CanvasLayer/WorkspacePosition, "modulate",
		Color(1, 1, 1, 1), Color(1, 1, 1, 0), 1,
		Tween.TRANS_LINEAR, Tween.EASE_IN_OUT)
	tween.start()
	yield(tween, "tween_completed")
	tween.queue_free()

func round_to_dec(num, digit):
	return round(num * pow(10.0, digit)) / pow(10.0, digit)

func get_workspace_mpos():
	return get_global_mouse_position()
