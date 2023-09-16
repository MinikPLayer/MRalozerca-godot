using Godot;
using System;

public class Fish : Area2D
{
    [Export] public float RotationSpeed = 360.0f;
    [Export] public new float Gravity = 98.1f;

    public FishSpawner Spawner;

    private float _currentSpeed = 0.0f;

    public override void _Process(float delta)
    {
        base._Process(delta);

        GetChild<Node2D>(0).Rotate(Mathf.Deg2Rad(RotationSpeed * delta));

        _currentSpeed += Gravity * delta;
        MoveLocalY(_currentSpeed * delta);

        GameManager.OnGameOver += () => Destroy(false);
        GameManager.OnFishCollected += (f, c) =>
        {
            if(f == this)
                Destroy(c);
        };
    }

    void Destroy(bool collected)
    {
        this.QueueFree();
    }
}
