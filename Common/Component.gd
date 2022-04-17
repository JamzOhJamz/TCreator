extends Control

var shadow_node = null
var dragPoint = null
var tModCreator

# Called when the node enters the scene tree for the first time.
func _ready():
	tModCreator = get_node("/root/tModCreator")

func _on_Panel_gui_input(event):
	if event is InputEventMouseButton:
		if event.button_index == BUTTON_LEFT:
			if event.pressed:
				# Grab it.
				if dragPoint == null:
					shadow_node = load("res://Common/Component.tscn").instance()
					shadow_node.modulate = Color(1, 1, 1, 0.5)
					shadow_node.rect_position.y = rect_global_position.y
					tModCreator.get_node("CanvasLayer").add_child(shadow_node)
				dragPoint = get_global_mouse_position() - shadow_node.get_position()
				tModCreator.draggingComponent = true
			else:
				# Release it and create new node on map
				var camera_node = tModCreator.get_node("Camera2D")
				var new_blueprint_node = load("res://Blueprints/Item/SetStaticDefaultsNode.tscn").instance()
				new_blueprint_node.rect_position = tModCreator.get_workspace_mpos()
				tModCreator.get_node("Workspace").add_child(new_blueprint_node)
				dragPoint = null
				shadow_node.queue_free()
				shadow_node = null
				tModCreator.draggingComponent = false
	if event is InputEventMouseMotion and dragPoint != null:
		if shadow_node != null:
			shadow_node.set_position(get_global_mouse_position() - dragPoint)
