using Godot;
using System;

public class CameraManager : Camera2D
{
    [Export]
    public NodePath TopPath;

    [Export] public float LostShakeDuration = 0.5f;
    [Export] public float CollectedShakeDuration = 0f;

    [Export] public float ShakeMultiplier = 30.0f;

    private float _currentShake = 0.0f;

    private void FixCamera(Vector2 offset)
    {
        var viewport = GetViewport();
        var topNode = GetNode<Node2D>(TopPath);
        var newPosY = topNode.GlobalPosition.y + viewport.Size.y / 2;

        var newPos = new Vector2(0, newPosY);
        GlobalPosition = newPos + offset;
    }

    private void OnFishCollected(Fish _, bool collected)
    {
        _currentShake = collected ? CollectedShakeDuration : LostShakeDuration;
    }

    public override void _Ready()
    {
        base._Ready();

        this.GetManager().Connect(nameof(GameManager.OnFishCollected), this, nameof(OnFishCollected));
    }

    public override void _Process(float delta)
    {
        var offset = new Vector2(
            (float)GD.RandRange(-_currentShake, _currentShake) * ShakeMultiplier,
            (float)GD.RandRange(-_currentShake, _currentShake) * ShakeMultiplier
        );

        _currentShake = Mathf.Max(0.0f, _currentShake - delta);
        FixCamera(offset);
    }
}
