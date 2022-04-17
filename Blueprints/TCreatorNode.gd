extends Control

var inConnections = []
var outConnections = []
var dragPoint = null
var line2d = null
var curve = null
var tModCreator

# Called when the node enters the scene tree for the first time.
func _ready():
	tModCreator = get_node("/root/tModCreator")

func _process(delta):
	if get_node_or_null("InNub") != null && tModCreator.nodeConnectionLineDrawing:
		var rect = Rect2($InNub.rect_global_position, $InNub.rect_size)
		if rect.has_point(tModCreator.get_workspace_mpos()):
			$InNub.get("custom_styles/panel").set_bg_color(Color("ec3b5a"))
			tModCreator.connectToInNub = $InNub
		else:
			$InNub.get("custom_styles/panel").set_bg_color(Color("ffffff"))
			tModCreator.connectToInNub = null
	if line2d != null && tModCreator.nodeConnectionLineDrawing:
		line2d.points[1] = line2d.to_local(tModCreator.get_workspace_mpos())
		#curve.clear_points()
		#curve.add_point(Vector2(4, 4), Vector2(0, 0), Vector2(70, 0))
		#curve.add_point(line2d.to_local(tModCreator.get_workspace_mpos()), Vector2(-70, 0), Vector2(0, 0))
		#line2d.set_points(curve.get_baked_points())

func _on_HeaderBar_gui_input(event):
	if event is InputEventMouseButton:
		if event.button_index == BUTTON_LEFT:
			if event.pressed:
				# Grab it.
				dragPoint = get_global_mouse_position() - get_position()
				tModCreator.draggingComponent = true
			else:
				# Release it.
				dragPoint = null
				tModCreator.draggingComponent = false
	if event is InputEventMouseMotion and dragPoint != null:
		set_position(get_global_mouse_position() - dragPoint)

func _on_OutNub_mouse_entered():
	$OutNub.get("custom_styles/panel").set_bg_color(Color("ec3b5a"))

func _on_OutNub_mouse_exited():
	$OutNub.get("custom_styles/panel").set_bg_color(Color("ffffff"))

func _on_OutNub_gui_input(event):
	if (event is InputEventMouseButton && event.button_index == 1):
		if event.pressed:
			line2d = Line2D.new()
			$OutNub.add_child(line2d)
			line2d.z_index = -1
			line2d.width = 2
			line2d.default_color = Color("ffffff")
			line2d.add_point(Vector2(4, 4))
			line2d.add_point(line2d.to_local(tModCreator.get_workspace_mpos()))
			tModCreator.draggingComponent = true
			tModCreator.nodeConnectionLineDrawing = true
		else:
			tModCreator.draggingComponent = false
			tModCreator.nodeConnectionLineDrawing = false
			if tModCreator.connectToInNub != null:
				curve = Curve2D.new()
				curve.add_point(Vector2(4, 4), Vector2(0, 0), Vector2(70, 0))
				curve.add_point(line2d.to_local(tModCreator.connectToInNub.rect_global_position + Vector2(4, 4)), Vector2(-70, 0), Vector2(0, 0))
				line2d.set_points(curve.get_baked_points())
				tModCreator.connectToInNub.get_parent().inConnections.append(self)
				outConnections.append(tModCreator.connectToInNub.get_parent())
				tModCreator.connectToInNub.get("custom_styles/panel").set_bg_color(Color("ffffff"))
				tModCreator.connectToInNub = null
			else:
				line2d.queue_free()
				line2d = null
	if event is InputEventMouseMotion and line2d != null:
		$OutNub.get("custom_styles/panel").set_bg_color(Color("ffffff"))





















