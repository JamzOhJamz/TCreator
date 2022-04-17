using Godot;

public class NodeConnectionLine : Line2D
{
    public BlueprintNode ParentNode;
    public BlueprintNode TargetNode;
    private Curve2D Curve;

	public NodeConnectionLine(Control parent, Control target)
    {
        parent.AddChild(this);
        this.ZIndex = -1;
        this.Width = 2;
        this.DefaultColor = new Color("ffffff");
        Curve = new Curve2D();
        Curve.AddPoint(new Vector2(4, 4), Vector2.Zero, new Vector2(70, 0));
        Curve.AddPoint(this.ToLocal(target.RectGlobalPosition + new Vector2(4, 4)), new Vector2(-70, 0), Vector2.Zero);
        this.Points = Curve.GetBakedPoints();
        ParentNode = parent.GetParent<BlueprintNode>();
        TargetNode = target.GetParent<BlueprintNode>();
    }

    public override void _Process(float delta)
    {
        if (!IsInstanceValid(TargetNode))
        {
            ParentNode.Branches.Remove(TargetNode);
            QueueFree();
            ParentNode.ConnectionLine = null;
        }
        if (ParentNode.DragPoint != null || TargetNode.DragPoint != null)
            RecalculatePoints();
    }

    private void RecalculatePoints()
    {
        Curve.ClearPoints();
        Curve.AddPoint(new Vector2(4, 4), Vector2.Zero, new Vector2(70, 0));
        Curve.AddPoint(this.ToLocal(TargetNode.GetNode<Control>("InNub").RectGlobalPosition + new Vector2(4, 4)), new Vector2(-70, 0), Vector2.Zero);
        this.Points = Curve.GetBakedPoints();
    }
}
