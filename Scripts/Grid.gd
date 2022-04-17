extends Node2D
export var on = true

const COLUMNS = 7
const ROWS = 6

func _draw():
	if on: 
		draw_rect(Rect2(0, 0, COLUMNS*100+20, ROWS*100+20), "#0000FF")
		for x in range(COLUMNS):
			for y in range(ROWS):
				draw_circle(Vector2(60+x*100, 60+y*100), 40, "#FFFFFF")

func _process(delta):
	update()
