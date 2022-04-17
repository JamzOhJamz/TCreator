using Godot;
using System.Collections.Generic;
using static Godot.Tween;

public class TCreator : Node2D
{
	public static bool InWorkspace = false;
	public static bool DraggingComponent = false;
	public static bool FocusedOnControl = false;
	public static bool NodeConnectionLineDrawing = false;
	public static List<Panel> ConnectToInNub = new List<Panel>();
	public static List<BlueprintNode> WorkspaceNodes = new List<BlueprintNode>();

	public override void _Ready()
	{
		OS.WindowMaximized = true;
	}

	public override void _Process(float delta)
	{
		if (GetNode<Control>("CanvasLayer/Control").GetFocusOwner() != null)
		{
			FocusedOnControl = true;
		} else
		{
			FocusedOnControl = false;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (!mouseEvent.IsPressed())
				return;
			if (mouseEvent.ButtonIndex != (int)ButtonList.Left && mouseEvent.ButtonIndex != (int)ButtonList.Right)
				return;
			Control focusOwner = GetNode<Control>("CanvasLayer/Control").GetFocusOwner();
			if (focusOwner == null)
				return;
			Rect2 rect = new Rect2(focusOwner.RectGlobalPosition, focusOwner.RectSize);
			if (rect.HasPoint(GetGlobalMousePosition()))
				return;
			//DeselectDropdowns();
			if (focusOwner is LineEdit focusedLineEdit)
			{
				focusedLineEdit.Deselect();
				focusedLineEdit.CaretPosition = 0;
			}
			CustomDropdown.DeselectDropdowns();
			focusOwner.ReleaseFocus();
		}
	}

	public void InitializeComponentsList()
	{
		Control generalList = GetNode<Control>("CanvasLayer/Sidebar/VBoxContainer/GeneralHeader");
		BlueprintComponent.CreateNew(generalList, "Set Static Defaults", new Color("ec3b5a"));
		BlueprintComponent.CreateNew(generalList, "Set Texture", new Color("ec3b5a"));
		Control typesList = GetNode<Control>("CanvasLayer/Sidebar/VBoxContainer/TypesHeader");
		BlueprintComponent.CreateNew(typesList, "Texture", new Color("3beca4"));
		Control otherList = GetNode<Control>("CanvasLayer/Sidebar/VBoxContainer/OtherHeader");
		BlueprintComponent.CreateNew(otherList, "Comment", new Color("a8a9bf"));
	}

	private void _on_NewProjectBtn_gui_input(object @event)
	{
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.Pressed && mouseEvent.ButtonIndex == (int)ButtonList.Left)
			{
				GD.Print("Clicked");
			}
		}
	}

	private void _on_NewProjectBtn_mouse_entered()
	{
		GetNode<Label>("CanvasLayer/Control/VBoxContainer/NewProjectBtn").Set("custom_colors/font_color", new Color("ff8197"));
	}

	private void _on_NewProjectBtn_mouse_exited()
	{
		GetNode<Label>("CanvasLayer/Control/VBoxContainer/NewProjectBtn").Set("custom_colors/font_color", new Color("ec3b5a"));
	}

	private void _on_NewBlueprintBtn_gui_input(object @event)
	{
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.Pressed && mouseEvent.ButtonIndex == (int)ButtonList.Left)
			{
				Control newBlueprintPopup = GetNode<Control>("CanvasLayer/NewBlueprintPopup");
				if (!newBlueprintPopup.Visible)
				{
					newBlueprintPopup.Visible = true;
				} else
				{
					newBlueprintPopup.Visible = false;
				}
			}
		}
	}

	private void _on_NewBlueprintBtn_mouse_entered()
	{
		GetNode<Label>("CanvasLayer/Control/VBoxContainer/NewBlueprintBtn").Set("custom_colors/font_color", new Color("ff8197"));
	}

	private void _on_NewBlueprintBtn_mouse_exited()
	{
		GetNode<Label>("CanvasLayer/Control/VBoxContainer/NewBlueprintBtn").Set("custom_colors/font_color", new Color("ec3b5a"));
	}

	public void SetNavbarText(string text)
	{
		GetNode<Label>("CanvasLayer/Navbar/TitleText").Text = text + " - TCreator Alpha";
	}

	public void CreateStartNodeInWorkspace()
	{
		Control startNode = GD.Load<PackedScene>("res://Blueprints/StartNode.tscn").Instance<Control>();
		startNode.RectPosition = new Vector2(0 - startNode.RectSize.x / 2, 0 - startNode.RectSize.y / 2);
		GetNode("Workspace").AddChild(startNode);
	}

	private Vector2 lastReceivedWorkspacePos;

	public async void ShowWorkspacePosition(Vector2 workspacePosition)
	{
		if (lastReceivedWorkspacePos == workspacePosition)
			return;
		lastReceivedWorkspacePos = workspacePosition;
		Label workspacePositionDisplay = GetNode<Label>("CanvasLayer/WorkspacePosition");
		workspacePositionDisplay.Modulate = new Color(1, 1, 1, 1);
		float roundedX = RoundToDec(workspacePosition.x, 1);
		float roundedY = RoundToDec(workspacePosition.y, 1);
		workspacePositionDisplay.Text = "[" + roundedX + ", " + roundedY + "]";
		Tween tween = new Tween();
		AddChild(tween);
		tween.InterpolateProperty(workspacePositionDisplay, "modulate",
			new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), 1,
			TransitionType.Linear, EaseType.InOut);
		tween.Start();
		await ToSignal(tween, "tween_completed");
		tween.QueueFree();
	}

	private float RoundToDec(float num, int digit)
	{
		return Mathf.Round(num * Mathf.Pow(10f, digit) / Mathf.Pow(10f, digit));
	}

	public Vector2 GetWorkspaceMpos()
	{
		return GetGlobalMousePosition();
	}
}
