using Godot;
using System;
using MRalozerca2.Scripts;

public class Fish : Area2D
{
    [Export] public float RotationSpeed = 360.0f;
    [Export] public new float Gravity = 98.1f;

    [Export] public NodePath InsideSpritePath;

    public bool IsCollected = false;

    public FishSpawner Spawner;
    public GameColors Color = ColorManager.RandomGameColor();

    private float _currentSpeed = 0.0f;
    private Sprite _insideSprite;

    public override void _Ready()
    {
        base._Ready();

        var gm = this.GetManager();
        gm.Connect(nameof(GameManager.OnGameOver), this, nameof(Destroy));
        gm.Connect(nameof(GameManager.OnFishCollected), this, nameof(OnFishCollected));
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (!this.GetManager().IsGameRunning)
            return;

        #if DEBUG
            if(Spawner == null)
                throw new NullReferenceException("Spawner is null! (Forgot to set?)");
        #endif

        _insideSprite = GetNode<Sprite>(InsideSpritePath);

        GetChild<Node2D>(0).Rotate(Mathf.Deg2Rad(RotationSpeed * delta));

        _currentSpeed += Gravity * delta * this.GetManager().GameSpeedMultiplier;
        MoveLocalY(_currentSpeed * delta);

        _insideSprite.Modulate = Color.ToColor();
    }

    private void OnFishCollected(Fish f, bool collected)
    {
        if(f == this)
            Destroy();
    }

    private void Destroy()
    {
        IsCollected = true;
        QueueFree();
    }
}
