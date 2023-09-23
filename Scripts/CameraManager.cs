using Godot;
using System;

public class CameraManager : Camera2D
{
    [Export]
    public NodePath TopPath;

    [Export] public float LostShakeDuration = 0.5f;
    [Export] public float CollectedShakeDuration = 0f;

    [Export] public float LostVibrationDuration = 0.3f;
    [Export] public float CollectedVibrationDuration = 0.1f;

    [Export] public float ShakeMultiplier = 30.0f;

    private float _currentShake = 0.0f;

    private bool _isGameStarted = false;

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
        if (collected)
        {
            Input.VibrateHandheld((int)(CollectedVibrationDuration * 1000));
            _currentShake = CollectedShakeDuration;
        }
        else
        {
            Input.VibrateHandheld((int)(LostVibrationDuration * 1000));
            _currentShake = LostShakeDuration;
        }
    }

    public override void _Ready()
    {
        base._Ready();

        this.GetManager().Connect(nameof(GameManager.OnFishCollected), this, nameof(OnFishCollected));
        this.GetManager().Connect(nameof(GameManager.OnGameStart), this, nameof(OnGameStart));
    }

    private void OnGameStart()
    {
        _isGameStarted = true;
    }

    public override void _Process(float delta)
    {
        if (!_isGameStarted)
            return;

        var offset = new Vector2(
            (float)GD.RandRange(-_currentShake, _currentShake) * ShakeMultiplier,
            (float)GD.RandRange(-_currentShake, _currentShake) * ShakeMultiplier
        );

        _currentShake = Mathf.Max(0.0f, _currentShake - delta);
        FixCamera(offset);
    }
}
