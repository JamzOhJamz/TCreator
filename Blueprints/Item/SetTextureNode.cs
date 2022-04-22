using Godot;
using Godot.Collections;
using Newtonsoft.Json;
using static Godot.Tween;

[JsonObject(MemberSerialization.OptIn)]
public class SetTextureNode : BlueprintNode
{
	[JsonProperty]
	public byte[] Data { get; set; }

	[JsonProperty]
	public int Width { get; set; }

	[JsonProperty]
	public int Height { get; set; }

	public override void PostPopulate()
	{
		LoadTexture(Data);
		FinishedLoading = true;
	}

	private ImageTexture Tex;
	private Color TweenColor = new Color("444558");

	private async void _on_TextureContainer_mouse_entered()
	{
		Tween tween = new Tween();
		AddChild(tween);
		tween.InterpolateProperty(this, "TweenColor",
			TweenColor, new Color("ec3b5a"), 0.15f,
			TransitionType.Linear, EaseType.InOut);
		tween.Start();
		await ToSignal(tween, "tween_completed");
		tween.QueueFree();
	}

	private async void _on_TextureContainer_mouse_exited()
	{
		Tween tween = new Tween();
		AddChild(tween);
		tween.InterpolateProperty(this, "TweenColor",
			TweenColor, new Color("444558"), 0.15f,
			TransitionType.Linear, EaseType.InOut);
		tween.Start();
		await ToSignal(tween, "tween_completed");
		tween.QueueFree();
	}

	public override void _Process(float delta)
	{
		base._Process(delta);
		GetNode<Label>("VBoxContainer/LoadPathBox/TextureContainer/TitleText").Set("custom_colors/font_color", TweenColor);
	}

	// Option 1: Load from GUI open file dialog
	private void _on_TextureContainer_gui_input(object @event)
	{
		if (Tex != null) 
			return;
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.Pressed && mouseEvent.ButtonIndex == (int)ButtonList.Left)
			{
				TCreatorInstance.ShowOpenFileDialog(LoadTexture, new string[] { "*.png ; PNG Images" });
			}
		}
	}

	// Option 2: Load from drag and drop
	public void OnFileDropped(Vector2 atPosition, string file, int screen)
	{
		// Check if the files were dropped on this component
		Panel loadPathBox = GetNode<Panel>("VBoxContainer/LoadPathBox");
		Panel textureContainer = loadPathBox.GetNode<Panel>("TextureContainer");
		Rect2 rect = new Rect2(textureContainer.RectGlobalPosition, textureContainer.RectSize);
		if (!rect.HasPoint(atPosition))
			return;
		LoadTexture(file);
	}

	public void LoadTexture(string file)
	{
		// Load texture
		Panel loadPathBox = GetNode<Panel>("VBoxContainer/LoadPathBox");
		Panel textureContainer = loadPathBox.GetNode<Panel>("TextureContainer");
		textureContainer.MouseDefaultCursorShape = CursorShape.Arrow;
		Label label = GetNode<Label>("VBoxContainer/LoadPathBox/TextureContainer/TitleText");
		label.Set("custom_colors/font_color", new Color("ffffff"));
		label.Text = "";
		CenterContainer centerContainer = textureContainer.GetNode<CenterContainer>("CenterContainer");
		TextureRect textureRect = centerContainer.GetNode<TextureRect>("TextureRect");
		Image image = new Image();
		image.Load(file);
		Tex = new ImageTexture();
		Tex.CreateFromImage(image, 3);
		Dictionary internalData = Tex.GetData().Data;
		Data = (byte[])internalData["data"];
		Height = (int)internalData["height"];
		Width = (int)internalData["width"];
		textureRect.Texture = Tex;
		if (Height > 53)
		{
			textureContainer.RectSize = new Vector2(textureContainer.RectSize.x, Height + 10);
			loadPathBox.RectMinSize = new Vector2(loadPathBox.RectSize.x, 70 + (Height - 53) + 10);
		}
		else
		{
			textureContainer.RectSize = new Vector2(209, 53);
			loadPathBox.RectMinSize = new Vector2(229, 70);
		}
		textureContainer.GetNode<TextureButton>("TextureResetButton").Show();
	}

	public void LoadTexture(byte[] data)
	{
		// Load texture
		Panel loadPathBox = GetNode<Panel>("VBoxContainer/LoadPathBox");
		Panel textureContainer = loadPathBox.GetNode<Panel>("TextureContainer");
		textureContainer.MouseDefaultCursorShape = CursorShape.Arrow;
		Label label = GetNode<Label>("VBoxContainer/LoadPathBox/TextureContainer/TitleText");
		label.Set("custom_colors/font_color", new Color("ffffff"));
		label.Text = "";
		CenterContainer centerContainer = textureContainer.GetNode<CenterContainer>("CenterContainer");
		TextureRect textureRect = centerContainer.GetNode<TextureRect>("TextureRect");
		Image image = new Image();
		image.CreateFromData(Width, Height, false, Image.Format.Rgba8, Data);
		Tex = new ImageTexture();
		Tex.CreateFromImage(image, 3);
		Dictionary internalData = Tex.GetData().Data;
		Data = (byte[])internalData["data"];
		Height = (int)internalData["height"];
		Width = (int)internalData["width"];
		textureRect.Texture = Tex;
		if (Height > 53)
		{
			textureContainer.RectSize = new Vector2(textureContainer.RectSize.x, Height + 10);
			loadPathBox.RectMinSize = new Vector2(loadPathBox.RectSize.x, 70 + (Height - 53) + 10);
		}
		else
		{
			textureContainer.RectSize = new Vector2(209, 53);
			loadPathBox.RectMinSize = new Vector2(229, 70);
		}
		textureContainer.GetNode<TextureButton>("TextureResetButton").Show();
	}

	private void _on_TextureResetButton_pressed()
	{
		Label label = GetNode<Label>("VBoxContainer/LoadPathBox/TextureContainer/TitleText");
		label.Set("custom_colors/font_color", new Color("444558"));
		label.Text = "Click to select from files...";
		Panel loadPathBox = GetNode<Panel>("VBoxContainer/LoadPathBox");
		Panel textureContainer = loadPathBox.GetNode<Panel>("TextureContainer");
		textureContainer.MouseDefaultCursorShape = CursorShape.PointingHand;
		CenterContainer centerContainer = textureContainer.GetNode<CenterContainer>("CenterContainer");
		TextureRect textureRect = centerContainer.GetNode<TextureRect>("TextureRect");
		textureRect.Texture = null;
		Tex = null;
		Data = null;
		Height = 0;
		Width = 0;
		textureContainer.RectSize = new Vector2(209, 53);
		loadPathBox.RectMinSize = new Vector2(229, 70);
		textureContainer.GetNode<TextureButton>("TextureResetButton").Hide();
	}
}
