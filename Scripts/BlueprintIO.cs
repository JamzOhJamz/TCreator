using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Godot.File;

public static class BlueprintIO
{
    public static bool OpeningTCRFile = false;

    public static void SaveTCRFile(string path)
    {
        JArray nodesProperty = new JArray();
        foreach (BlueprintNode node in TCreator.WorkspaceNodes)
        {
            if (node is CommentNode commentNode)
                nodesProperty.Add(JToken.FromObject(commentNode));
            if (node is SetStaticDefaultsNode setStaticDefaultsNode)
                nodesProperty.Add(JToken.FromObject(setStaticDefaultsNode));
            if (node is SetTextureNode setTextureNode)
                nodesProperty.Add(JToken.FromObject(setTextureNode));
            if (node is TextureNode textureNode)
                nodesProperty.Add(JToken.FromObject(textureNode));
        }
        JObject fileData = new JObject(
            new JProperty("CPosX", TCreator.CameraPosition.x),
            new JProperty("CPosY", TCreator.CameraPosition.y),
            new JProperty("CZoom", TCreator.CameraZoom),
            new JProperty("Nodes", nodesProperty));
        File file = new File();
        file.Open(path, ModeFlags.Write);
        file.StoreString(fileData.ToString());
        file.Close();
        TCreator.Instance.SetNavbarText(System.IO.Path.GetFileName(path));
        //OS.ShellOpen(System.IO.Path.GetDirectoryName(path));
    }

    public static async void OpenTCRFile(string path)
    {
        //TO-DO: Add a prompt to confirm blueprint load + overwrite.
        if (OpeningTCRFile) return;
        OpeningTCRFile = true;
        List<BlueprintNode> oldWorkspaceNodes = new List<BlueprintNode>(TCreator.WorkspaceNodes);
        File file = new File();
        file.Open(path, ModeFlags.Read);
        JObject fileData = JObject.Parse(file.GetAsText());
        JArray nodesProperty = (JArray)fileData.SelectToken("Nodes");
        foreach (JObject node in nodesProperty)
        {
            string typeString = (string)node.SelectToken("Type");
            Type t = Type.GetType(typeString + "Node");
            BlueprintNode newComponent = GD.Load<PackedScene>($"res://Blueprints/Item/{typeString}Node.tscn").Instance<BlueprintNode>();
            newComponent.Type = typeString;
            newComponent.RectPosition = BlueprintNode.OffScreenPosition;
            newComponent.PostLoadSetPosition = new Vector2((float)node.SelectToken("WorkspacePosition.x"), (float)node.SelectToken("WorkspacePosition.y"));
            TCreator.Instance.GetNode("Workspace").AddChild(newComponent);
            object castComponent = Convert.ChangeType(newComponent, t);
            JsonConvert.PopulateObject(node.ToString(), castComponent);
            newComponent.PostPopulate();
        }
        while (!TCreator.WorkspaceNodes.All(x => x.FinishedLoading == true))
            await Task.Delay(25);
        foreach (BlueprintNode node in oldWorkspaceNodes)
            node.QueueFree();
        foreach (BlueprintNode node in TCreator.WorkspaceNodes)
            node.RectGlobalPosition = node.PostLoadSetPosition;
        Vector2 newCamPos = new Vector2((float)fileData.SelectToken("CPosX"), (float)fileData.SelectToken("CPosY"));
        TCreator.SetCameraPosition(newCamPos, true);
        TCreator.Instance.SetLastReceivedWorkspacePosition(newCamPos);
        // ignore loading zoom for right now 
        // TCreator.SetCameraZoom((float)fileData.SelectToken("CZoom"));
        TCreator.Instance.SetNavbarText(System.IO.Path.GetFileName(path));
        OpeningTCRFile = false;
        TCreator.Instance.HideOpenFileDialog();
    }
}
