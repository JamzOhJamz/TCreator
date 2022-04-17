using Godot;

public class BlueprintComponent : Control
{
	private string DisplayName;
	private Color Color;
	private string Type;
	private BlueprintComponent ShadowNode = null;
	private Vector2? DragPoint = null;
	private TCreator TCreator;

	public static void CreateNew(Control list, string displayName, Color color)
	{
		BlueprintComponent component = GD.Load<PackedScene>("res://Common/Component.tscn").Instance<BlueprintComponent>();
		list.AddChild(component);
		component.SetDisplayName(displayName);
		component.SetColor(color);
	}

	public override void _Ready()
	{
		TCreator = GetNode<TCreator>("/root/tModCreator");
		StyleBoxFlat panelStyle = GetNode<Panel>("Centerer/Panel").Get("custom_styles/panel") as StyleBoxFlat;
		GetNode<Panel>("Centerer/Panel").Set("custom_styles/panel", panelStyle.Duplicate());
	}

	public void SetDisplayName(string name)
	{
		DisplayName = name;
		Type = name.Replace(" ", "");
		GetNode<Label>("Centerer/Panel/Text").Text = name;
	}

	public void SetColor(Color color)
	{
		Color = color;
		StyleBoxFlat panelStyle = GetNode<Panel>("Centerer/Panel").Get("custom_styles/panel") as StyleBoxFlat;
		panelStyle.BorderColor = color;
	}

	public void SetType(string type)
	{
		Type = type;
	}

	private void _on_Panel_gui_input(object @event)
	{
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.ButtonIndex != (int)ButtonList.Left)
				return;
			if (mouseEvent.Pressed)
			{
				// Grab it
				if (DragPoint == null)
				{
					ShadowNode = GD.Load<PackedScene>("res://Common/Component.tscn").Instance<BlueprintComponent>();
					ShadowNode.SetDisplayName(DisplayName);
					ShadowNode.SetColor(Color);
					ShadowNode.Modulate = new Color(1, 1, 1, 0.5f);
					ShadowNode.RectPosition = new Vector2(ShadowNode.RectPosition.x, RectGlobalPosition.y);
					TCreator.GetNode("CanvasLayer").AddChild(ShadowNode);
				}
				DragPoint = GetGlobalMousePosition() - ShadowNode.RectPosition;
				TCreator.DraggingComponent = true;
			} else
			{
				// Release it and create new node on map
				Camera2D cam = TCreator.GetNode<Camera2D>("Camera2D");
				Control newComponent = GD.Load<PackedScene>($"res://Blueprints/Item/{Type}Node.tscn").Instance<Control>();
				newComponent.RectPosition = TCreator.GetWorkspaceMpos();
				TCreator.GetNode("Workspace").AddChild(newComponent);
				ShadowNode.QueueFree();
				ShadowNode = null;
				DragPoint = null;
				TCreator.DraggingComponent = false;
			}
		}
		if (@event is InputEventMouseMotion motionEvent && DragPoint != null)
		{
			if (ShadowNode != null)
				ShadowNode.RectPosition = GetGlobalMousePosition() - (Vector2)DragPoint;
		}
	}
}
