using Godot;

public class NewBlueprintOption : Panel
{
	private TCreator TCreator;

	public override void _Ready()
	{
		TCreator = GetNode<TCreator>("/root/tModCreator");
	}

	private void _on_NewBlueprintOption_mouse_entered()
	{
		StyleBoxFlat panelStyle = Get("custom_styles/panel") as StyleBoxFlat;
		panelStyle.BgColor = new Color("1c1c24");
	}

	private void _on_NewBlueprintOption_mouse_exited()
	{
		StyleBoxFlat panelStyle = Get("custom_styles/panel") as StyleBoxFlat;
		panelStyle.BgColor = new Color("16161d");
	}

	private void _on_NewBlueprintOption_gui_input(object @event)
	{
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.Pressed && mouseEvent.ButtonIndex == (int)ButtonList.Left)
			{
				TCreator.GetNode<Control>("CanvasLayer/NewBlueprintPopup").Hide();
				TCreator.GetNode<Control>("CanvasLayer/Control").Hide();
				TCreator.GetNode<Control>("CanvasLayer/Sidebar").Show();
				TCreator.SetNavbarText("Untitled-1");
				//TCreator.CreateStartNodeInWorkspace();
				TCreator.InitializeComponentsList();
				TCreator.InWorkspace = true;
			}
		}
	}
}
