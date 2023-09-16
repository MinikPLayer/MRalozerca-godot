using Godot;
using System;

public class CameraManager : Camera2D
{
    [Export]
    public NodePath TopPath;

    void FixCamera()
    {
        var viewport = GetViewport();
        var topNode = GetNode<Node2D>(TopPath);
        var newPosY = topNode.GlobalPosition.y + viewport.Size.y / 2;

        var newPos = new Vector2(GlobalPosition.x, newPosY);
        GlobalPosition = newPos;
    }


    public override void _Process(float delta)
    {
        FixCamera();
    }
}
