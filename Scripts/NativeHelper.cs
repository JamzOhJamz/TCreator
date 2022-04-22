using Godot;

public class NativeHelper : Node
{
    private TCreator TCreator;

    public override void _Ready()
    {
        TCreator = GetNode<TCreator>("/root/tModCreator");
        GetTree().Connect("files_dropped", this, nameof(OnFilesDropped));
    }

    public void OnFilesDropped(string[] files, int screen)
    {
        switch (System.IO.Path.GetExtension(files[0]))
        {
            case ".tcr":
                BlueprintIO.OpenTCRFile(files[0]);
                break;
            case ".png":
                Vector2 workspaceMpos = TCreator.GetWorkspaceMpos();
                foreach (BlueprintNode node in TCreator.WorkspaceNodes)
                {
                    if (node is TextureNode textureNode)
                        textureNode.OnFileDropped(workspaceMpos, files[0], screen);
                    if (node is SetTextureNode setTextureNode)
                        setTextureNode.OnFileDropped(workspaceMpos, files[0], screen);
                }
                break;
            default:
                break;
        }
    }
}
