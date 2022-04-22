using Godot;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System.Collections.Generic;

[JsonObject(MemberSerialization.OptIn)]
public class BlueprintNode : Control
{
	[JsonProperty]
	public string Type { get; set; }

	[JsonProperty]
	public Vector2 WorkspacePosition { get { return RectGlobalPosition; } }

	public static Vector2 OffScreenPosition = -Vector2.One * 99999999f;
	public Vector2 PostLoadSetPosition;
	public List<BlueprintNode> Branches = new List<BlueprintNode>();
	public Vector2? DragPoint = null;
	public NodeConnectionLine ConnectionLine = null;
	public TCreator TCreatorInstance;
	public bool FinishedLoading = false;
	private Line2D TempDrawLine = null;

	public override void _Ready()
	{
		TCreatorInstance = GetNode<TCreator>("/root/tModCreator");
		TCreator.WorkspaceNodes.Add(this);
		Panel inNub = GetNodeOrNull<Panel>("InNub");
		if (inNub != null)
		{
			StyleBoxFlat inNubPanelStyle = inNub.Get("custom_styles/panel") as StyleBoxFlat;
			GetNode<Panel>("InNub").Set("custom_styles/panel", inNubPanelStyle.Duplicate());
		}
		Panel outNub = GetNodeOrNull<Panel>("OutNub");
		if (outNub != null)
		{
			StyleBoxFlat outNubPanelStyle = outNub.Get("custom_styles/panel") as StyleBoxFlat;
			GetNode<Panel>("OutNub").Set("custom_styles/panel", outNubPanelStyle.Duplicate());
		}
	}

	public override void _Process(float delta)
	{
		Panel inNub = GetNodeOrNull<Panel>("InNub");
		if (inNub != null && TCreator.NodeConnectionLineDrawing)
		{
			Rect2 rect = new Rect2(inNub.RectGlobalPosition, inNub.RectSize);
			StyleBoxFlat panelStyle = inNub.Get("custom_styles/panel") as StyleBoxFlat;
			if (rect.HasPoint(TCreatorInstance.GetWorkspaceMpos()))
			{
				panelStyle.BgColor = new Color("ec3b5a");
				TCreator.ConnectToInNub.Add(inNub);
			}
			else
			{
				panelStyle.BgColor = new Color("ffffff");
				TCreator.ConnectToInNub.Remove(inNub);
			}
		}
		if (TempDrawLine != null && TCreator.NodeConnectionLineDrawing)
			TempDrawLine.SetPoints(new Vector2[2] { TempDrawLine.Points[0], TempDrawLine.ToLocal(TCreatorInstance.GetWorkspaceMpos()) });
	}

	public override void _Notification(int what)
	{
		if (what == NotificationPredelete)
			TCreator.WorkspaceNodes.Remove(this);
	}

	private void _on_HeaderBar_gui_input(object @event)
	{
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.ButtonIndex != (int)ButtonList.Left)
				return;
			if (mouseEvent.Pressed)
			{
				DragPoint = GetGlobalMousePosition() - RectPosition;
				TCreator.DraggingComponent = true;
			}
			else
			{
				DragPoint = null;
				TCreator.DraggingComponent = false;
			}
		}
		if (@event is InputEventMouseMotion motionEvent && DragPoint != null)
		{
			RectPosition = GetGlobalMousePosition() - (Vector2)DragPoint;
		}
	}
	
	private void _on_DeleteButton_pressed()
	{
		QueueFree();
	}

	private void _on_OutNub_mouse_entered()
	{
		StyleBoxFlat panelStyle = GetNode<Panel>("OutNub").Get("custom_styles/panel") as StyleBoxFlat;
		panelStyle.BgColor = new Color("ec3b5a");
	}

	private void _on_OutNub_mouse_exited()
	{
		StyleBoxFlat panelStyle = GetNode<Panel>("OutNub").Get("custom_styles/panel") as StyleBoxFlat;
		panelStyle.BgColor = new Color("ffffff");
	}

	private bool OutNubClicked = false;

	private void _on_OutNub_gui_input(object @event)
	{
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.ButtonIndex != (int)ButtonList.Left)
				return;
			if (mouseEvent.Pressed)
			{
				OutNubClicked = true;
				TempDrawLine = new Line2D();
				GetNode("OutNub").AddChild(TempDrawLine);
				TempDrawLine.ZIndex = -1;
				TempDrawLine.Width = 2;
				TempDrawLine.DefaultColor = new Color("ffffff");
				TempDrawLine.AddPoint(new Vector2(4, 4));
				TempDrawLine.AddPoint(TempDrawLine.ToLocal(TCreatorInstance.GetWorkspaceMpos()));
				TCreator.DraggingComponent = true;
				TCreator.NodeConnectionLineDrawing = true;
			}
			else
			{
				if (OutNubClicked && ConnectionLine != null)
				{
					Branches.Remove(ConnectionLine.TargetNode);
					ConnectionLine.QueueFree();
					ConnectionLine = null;
				}
				TCreator.DraggingComponent = false;
				TCreator.NodeConnectionLineDrawing = false;
				if (TCreator.ConnectToInNub.Count > 0)
				{
					if (ConnectionLine != null)
					{
						Branches.Remove(ConnectionLine.TargetNode);
						ConnectionLine.QueueFree();
						ConnectionLine = null;
					}
					ConnectionLine = new NodeConnectionLine(GetNode<Control>("OutNub"), TCreator.ConnectToInNub[0]);
					Branches.Add(TCreator.ConnectToInNub[0].GetParent<BlueprintNode>());
					StyleBoxFlat panelStyle = TCreator.ConnectToInNub[0].Get("custom_styles/panel") as StyleBoxFlat;
					panelStyle.BgColor = new Color("ffffff");
					TCreator.ConnectToInNub.Clear();
				}
				TempDrawLine.QueueFree();
				TempDrawLine = null;
			}
		}
		if (@event is InputEventMouseMotion && TempDrawLine != null)
		{
			OutNubClicked = false;
			StyleBoxFlat panelStyle = GetNode<Panel>("OutNub").Get("custom_styles/panel") as StyleBoxFlat;
			panelStyle.BgColor = new Color("ffffff");
		}
	}

	/// <summary>
	/// Called after TCR file loading populates this node's JSON properties
	/// </summary>
	public virtual void PostPopulate() { }

	/// <summary>
	/// Called when code is being exported for this node's blueprint
	/// </summary>
	public virtual void OnExport(ref ClassDeclarationSyntax mainClassDeclaration) { }

	/// <summary>
	/// Called after final export code is completely generated. Make final formatting changes here.
	/// </summary>
	/// <param name="mainClassDeclaration"></param>
	public virtual void PostExport(ref CompilationUnitSyntax compilationUnit) { }
}
