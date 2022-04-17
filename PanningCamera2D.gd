extends Camera2D
class_name PanningCamera2D

const MIN_ZOOM: float = 0.1
const MAX_ZOOM: float = 1.0
const ZOOM_RATE: float = 8.0
const ZOOM_INCREMENT: float = 0.1

var _target_zoom: float = 1.0

var tModCreator

func _ready():
	tModCreator = get_node("/root/tModCreator")

func _physics_process(delta: float) -> void:
	zoom = lerp(zoom, _target_zoom * Vector2.ONE, ZOOM_RATE * delta)
	set_physics_process(not is_equal_approx(zoom.x, _target_zoom))

func _input(event: InputEvent) -> void:
	if not tModCreator.inWorkspace:
		return
	if event is InputEventMouseButton:
		if event.is_pressed():
			if event.button_index == BUTTON_WHEEL_UP:
				zoom_in()
			if event.button_index == BUTTON_WHEEL_DOWN:
				zoom_out()
	if event is InputEventMouseMotion:
		if tModCreator.draggingComponent or tModCreator.focusedOnControl:
			return
		if event.button_mask == BUTTON_MASK_LEFT or event.button_mask == BUTTON_MASK_RIGHT:
			position -= event.relative * zoom
			tModCreator.show_workspace_position(position)

func zoom_in() -> void:
	_target_zoom = max(_target_zoom - ZOOM_INCREMENT, MIN_ZOOM)
	set_physics_process(true)

func zoom_out() -> void:
	_target_zoom = min(_target_zoom + ZOOM_INCREMENT, MAX_ZOOM)
	set_physics_process(true)
