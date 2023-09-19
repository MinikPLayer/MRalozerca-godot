using Godot;
using System;

public class CameraManager : Camera2D
{
    [Export]
    public NodePath TopPath;

    private DateTime _lastShakeTime = DateTime.Now;

    private void FixCamera(Vector2 offset)
    {
        var viewport = GetViewport();
        var topNode = GetNode<Node2D>(TopPath);
        var newPosY = topNode.GlobalPosition.y + viewport.Size.y / 2;

        var newPos = new Vector2(GlobalPosition.x, newPosY);
        GlobalPosition = newPos + offset;
    }

    private void OnFishCollected(Fish _, bool collected)
    {

    }

    public override void _Process(float delta)
    {
        FixCamera(Vector2.Zero);
    }
}
