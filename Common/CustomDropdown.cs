using Godot;
using System;
using System.Collections.Generic;

public class CustomDropdown : Control
{
	private static List<CustomDropdown> Dropdowns = new List<CustomDropdown>();

	public override void _Ready()
	{
		Dropdowns.Add(this);
	}

	public override void _Notification(int what)
	{
		if (what == NotificationPredelete)
			Dropdowns.Remove(this);
	}

	private void _on_OptionInput_gui_input(object @event)
	{
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.Pressed && mouseEvent.ButtonIndex == (int)ButtonList.Left)
			{
				DropdownItem mainPanel = GetNode<DropdownItem>("DropdownItem");
				if (!mainPanel.Visible)
				{
					mainPanel.Show();
				}
				else
				{
					mainPanel.Hide();
				}
			}
		}
	}

	public static void DeselectDropdowns()
	{
		foreach (CustomDropdown dropdown in Dropdowns)
		{
			foreach (var child in dropdown.GetChildren())
			{
				if (child is DropdownItem dropdownItem)
				{
					dropdownItem.Hide();
				}
			}
		}
	}
}
