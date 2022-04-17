using Godot;
using System.IO;

public class TextureNode : BlueprintNode
{
	public void OnFileDropped(Vector2 atPosition, string file, int screen)
	{
		// This entire method is painfully unoptimized and hardcoded. Fix it later... maybe.

		// Check if the files were dropped on this node
		Panel loadPathBox = GetNode<Panel>("VBoxContainer/LoadPathBox");
		Panel textureContainer = loadPathBox.GetNode<Panel>("TextureContainer");
		Rect2 rect = new Rect2(textureContainer.RectGlobalPosition, textureContainer.RectSize);
		if (!rect.HasPoint(atPosition)) 
			return;

		// Load texture
		LineEdit textureNameLabel = GetNode<LineEdit>("VBoxContainer/NameBox/StringInput");
		textureNameLabel.Text = System.IO.Path.GetFileNameWithoutExtension(file);
		Label label = GetNode<Label>("VBoxContainer/LoadPathBox/TextureContainer/TitleText");
		label.Set("custom_colors/font_color", new Color("ffffff"));
		label.Text = "";
		CenterContainer centerContainer = textureContainer.GetNode<CenterContainer>("CenterContainer");
		TextureRect textureRect = centerContainer.GetNode<TextureRect>("TextureRect");
		Image image = new Image();
		image.Load(file);
		ImageTexture tex = new ImageTexture();
		tex.CreateFromImage(image, 3);
		textureRect.Texture = tex;
		if (tex.GetHeight() > 53)
		{
			textureContainer.RectSize = new Vector2(textureContainer.RectSize.x, tex.GetHeight() + 10);
			loadPathBox.RectSize = new Vector2(loadPathBox.RectSize.x, 70 + (tex.GetHeight() - 53) + 10);
		}
		textureContainer.GetNode<TextureButton>("TextureResetButton").Show();
	}

	private void _on_TextureResetButton_pressed()
	{
		Label label = GetNode<Label>("VBoxContainer/LoadPathBox/TextureContainer/TitleText");
		label.Set("custom_colors/font_color", new Color("444558"));
		label.Text = "Drag and drop a file here";
		Panel loadPathBox = GetNode<Panel>("VBoxContainer/LoadPathBox");
		Panel textureContainer = loadPathBox.GetNode<Panel>("TextureContainer");
		CenterContainer centerContainer = textureContainer.GetNode<CenterContainer>("CenterContainer");
		TextureRect textureRect = centerContainer.GetNode<TextureRect>("TextureRect");
		textureRect.Texture = null;
		textureContainer.RectSize = new Vector2(209, 53);
		loadPathBox.RectSize = new Vector2(229, 70);
		textureContainer.GetNode<TextureButton>("TextureResetButton").Hide();
	}
}
