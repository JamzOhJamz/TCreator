using Godot;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class CommentNode : BlueprintNode
{
	[JsonProperty]
	public string CommentText
	{
		get { return TooltipEdit.Text; }
		set {
			TooltipEdit.Text = value;
		}
	}

	public override void PostPopulate()
	{
		_on_StringInput_text_changed();
		FinishedLoading = true;
	}

	Panel TooltipBox;
	TextEdit TooltipEdit;
	float TooltipBoxBaseHeight;

	public override void _Ready()
	{
		base._Ready();
		TooltipBox = GetNode<Panel>("VBoxContainer/TooltipBox");
		TooltipEdit = TooltipBox.GetNode<TextEdit>("StringInput");
		TooltipBoxBaseHeight = TooltipBox.RectSize.y;
	}

	private void _on_StringInput_text_changed()
	{
		int lines = TooltipEdit.Text.Split("\n").Length;
		int linesWithWrapping = lines - 1;
		for (var i = 0; i < lines; i++)
		{
			linesWithWrapping += TooltipEdit.GetLineWrapCount(i);
		}
		TooltipEdit.RectSize = new Vector2(TooltipEdit.RectSize.x, 21f + 18f * linesWithWrapping);
		TooltipBox.RectMinSize = new Vector2(TooltipBox.RectSize.x, TooltipBoxBaseHeight + 18f * linesWithWrapping);
	}
}
