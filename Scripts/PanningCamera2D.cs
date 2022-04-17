using Godot;

public class PanningCamera2D : Camera2D
{
	public float MIN_ZOOM = 0.1f;
	public float MAX_ZOOM = 2f;
	public float ZOOM_RATE = 8f;
	public float ZOOM_INCREMENT = 0.1f;

	private float TargetZoom = 0.999f;

	private TCreator TCreator;

	public override void _Ready()
	{
		TCreator = GetNode<TCreator>("/root/tModCreator");
	}

	public override void _PhysicsProcess(float delta)
	{
		float lerp = Mathf.Lerp(Zoom.x, TargetZoom, ZOOM_RATE * delta);
		Zoom = new Vector2(lerp, lerp);
		SetPhysicsProcess(!(Zoom.x == TargetZoom));
	}

	public override void _Input(InputEvent @event)
	{
		if (!TCreator.InWorkspace)
			return;
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (!mouseEvent.Pressed)
				return;
			if (mouseEvent.ButtonIndex == (int)ButtonList.WheelUp)
				ZoomIn();
			if (mouseEvent.ButtonIndex == (int)ButtonList.WheelDown)
				ZoomOut();
		}
		if (TCreator.DraggingComponent || TCreator.FocusedOnControl)
			return;
		if (@event is InputEventMouseMotion motionEvent)
		{
			if (motionEvent.ButtonMask == (int)ButtonList.MaskLeft || motionEvent.ButtonMask == (int)ButtonList.MaskRight)
			{
				Position -= motionEvent.Relative * Zoom;
				TCreator.ShowWorkspacePosition(Position);
			}
		}
	}

	private void ZoomIn()
	{
		TargetZoom = Mathf.Max(TargetZoom - ZOOM_INCREMENT, MIN_ZOOM);
		SetPhysicsProcess(true);
	}

	private void ZoomOut()
	{
		TargetZoom = Mathf.Min(TargetZoom + ZOOM_INCREMENT, MAX_ZOOM);
		SetPhysicsProcess(true);
	}
}
