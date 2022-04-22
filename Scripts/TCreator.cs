using Godot;
using System;
using System.Collections.Generic;
using static Godot.Tween;

public class TCreator : Node2D
{
	public static TCreator Instance;
	public static bool InWorkspace = false;
	public static bool InFileDialog = false;
	public static bool DraggingComponent = false;
	public static bool FocusedOnControl = false;
	public static bool NodeConnectionLineDrawing = false;
	public static List<Panel> ConnectToInNub = new List<Panel>();
	public static List<BlueprintNode> WorkspaceNodes = new List<BlueprintNode>();
	public static Vector2 CameraPosition;
	public static float CameraZoom = 0.999f;

	private RoslynCompiler CodeCompiler;

	public override void _Ready()
	{
		Instance = this;
		OS.WindowMaximized = true;
		GetTree().Connect("screen_resized", this, nameof(OnWindowSizeChanged));
		CodeCompiler = GetTree().Root.GetNode<RoslynCompiler>("RoslynCompiler");
	}

	private void OnWindowSizeChanged()
	{
		GetNode<FileDialog>("CanvasLayer/OpenFileDialog").Hide();
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
			if (focusOwner is TextEdit focusedTextEdit)
				focusedTextEdit.Deselect();
			CustomDropdown.DeselectDropdowns();
			focusOwner.ReleaseFocus();
		}
		// Handle shortcuts (eg. Ctrl+S)
		else if (@event is InputEventKey keyEvent)
		{
			if (!keyEvent.IsPressed())
				return;
			if (Input.IsKeyPressed((int)KeyList.Shift))
			{
				if (keyEvent.Scancode == (uint)KeyList.Enter)
					OS.WindowFullscreen = !OS.WindowFullscreen;
			}
			if (Input.IsKeyPressed((int)KeyList.Control))
			{
				if (keyEvent.Scancode == (uint)KeyList.S)
				{
					if (InWorkspace && WorkspaceNodes.Count > 0)
						ShowSaveFileDialog(BlueprintIO.SaveTCRFile);
				} else if (keyEvent.Scancode == (uint)KeyList.O)
				{
					if (InWorkspace)
						ShowOpenFileDialog(BlueprintIO.OpenTCRFile, new string[] { "*.tcr ; TCreator Blueprint" });
				} else if (keyEvent.Scancode == (uint)KeyList.E)
				{
					if (InWorkspace && WorkspaceNodes.Count > 0)
					{
						CodeCompiler.Export();
					}
				}
			}
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

	private Action<string> OpenFileCallback;

	public void ShowOpenFileDialog(Action<string> callback, string[] filters)
	{
		FileDialog fd = GetNode<FileDialog>("CanvasLayer/OpenFileDialog");
		fd.Filters = filters;
		fd.RectSize = new Vector2(OS.WindowSize.x, OS.WindowSize.y - 20);
		fd.PopupCentered();
		fd.Invalidate();
		InFileDialog = true;
		OpenFileCallback = callback;
	}

	public void HideOpenFileDialog()
	{
		FileDialog fd = GetNode<FileDialog>("CanvasLayer/OpenFileDialog");
		fd.Hide();
		if (!BlueprintIO.OpeningTCRFile)
		{
			InFileDialog = false;
			return;
		}
	}

	private void _on_OpenFileDialog_file_selected(string path)
	{
		if (OpenFileCallback != null)
		{
			OpenFileCallback.Invoke(path);
			OpenFileCallback = null;
		}
	}

	private void _on_OpenFileDialog_popup_hide()
	{
		if (!BlueprintIO.OpeningTCRFile)
		{
			InFileDialog = false;
			return;
		}
		FileDialog fd = GetNode<FileDialog>("CanvasLayer/OpenFileDialog");
		fd.Show();
	}

	private Action<string> SaveFileCallback;

	public void ShowSaveFileDialog(Action<string> callback)
	{
		FileDialog fd = GetNode<FileDialog>("CanvasLayer/SaveFileDialog");
		fd.RectSize = new Vector2(OS.WindowSize.x, OS.WindowSize.y - 20);
		fd.PopupCentered();
		fd.Invalidate();
		InFileDialog = true;
		SaveFileCallback = callback;
	}

	private void _on_SaveFileDialog_file_selected(string path)
	{
		if (SaveFileCallback != null)
		{
			SaveFileCallback.Invoke(path);
			SaveFileCallback = null;
		}
	}

	private void _on_SaveFileDialog_hide()
	{
		InFileDialog = false;
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
		Node richPresenceHandler = GetTree().Root.GetNode("RichPresenceHandler");
		richPresenceHandler.Call("_update_details", text);
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
			new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), 0.7f,
			TransitionType.Linear, EaseType.InOut);
		tween.Start();
		await ToSignal(tween, "tween_completed");
		tween.QueueFree();
	}

	public void SetLastReceivedWorkspacePosition(Vector2 workspacePosition) => lastReceivedWorkspacePos = workspacePosition;

	private float RoundToDec(float num, int digit)
	{
		return Mathf.Round(num * Mathf.Pow(10f, digit) / Mathf.Pow(10f, digit));
	}

	public Vector2 GetWorkspaceMpos()
	{
		return GetGlobalMousePosition();
	}

	public async static void SetCameraPosition(Vector2 pos, bool lerp)
	{
		CameraPosition = pos;
		if (!lerp)
		{
			Instance.GetNode<PanningCamera2D>("Camera2D").Position = pos;
			return;
		}
		Tween tween = new Tween();
		Instance.AddChild(tween);
		tween.InterpolateProperty(Instance.GetNode<PanningCamera2D>("Camera2D"), "position",
			Instance.GetNode<PanningCamera2D>("Camera2D").Position, pos, 0.5f,
			TransitionType.Expo, EaseType.Out);
		tween.Start();
		await Instance.ToSignal(tween, "tween_completed");
		tween.QueueFree();
	}

	public static void SetCameraZoom(float zoom)
	{
		CameraZoom = zoom;
		Instance.GetNode<PanningCamera2D>("Camera2D").TargetZoom = zoom;
		Instance.GetNode<PanningCamera2D>("Camera2D").SetPhysicsProcess(true);
	}
}
