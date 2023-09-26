using Godot;
using System;
using MRalozerca2.Scripts;

public class Fish : Area2D
{
    [Export] public float RotationSpeed = 360.0f;
    [Export] public new float Gravity = 98.1f;

    [Export] public NodePath InsideSpritePath;

    [Export] public PackedScene FishDestroyedParticles;

    public bool IsCollected = false;

    public FishSpawner Spawner;
    public GameColors Color = ColorManager.RandomGameColor();

    private float _currentSpeed = 0.0f;
    private Sprite _insideSprite;

    public MRal CollidedMRal = null;

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

        var speedMultiplier = this.GetManager().GameSpeedMultiplier;
        _currentSpeed += Gravity * delta * Mathf.Pow(speedMultiplier, 1.5f);
        MoveLocalY(_currentSpeed * delta);

        _insideSprite.Modulate = Color.ToColor();
    }

    private void SpawnParticles(float rightDirection)
    {
        var angle = GetChild<Node2D>(0).Rotation;

        var dirAngle = -angle - (rightDirection * Mathf.Pi / 2f);
        var y = Mathf.Cos(dirAngle);
        var x = -Mathf.Sin(dirAngle);

        var particles = (ParticlesManager)FishDestroyedParticles.Instance();

        particles.GlobalPosition = GlobalPosition;
        particles.GlobalRotation = angle;
        particles.OneShot = true;
        if(!(particles.ProcessMaterial is ParticlesMaterial pm))
            throw new InvalidCastException("ParticlesMaterial is not ParticlesMaterial!");

        pm.Direction = new Vector3(x, y, 0.0f);
        pm.InitialVelocity = _currentSpeed;
        pm.Spread = rightDirection == 0 ? 15f : 90f;

        particles.Emitting = true;
        GetTree().Root.AddChild(particles);
    }

    private void OnFishCollected(Fish f, bool collected)
    {
        if (f == this)
        {
            var move = 0f;
            if (collected && CollidedMRal != null)
            {
                var globalMove = CollidedMRal.CalculateGlobalMove(CollidedMRal.CurrentMove);
                var newMove = new Vector2(globalMove, _currentSpeed);
                var angle = newMove.Angle();
                move = Mathf.Cos(angle);
            }

            SpawnParticles(move);

            Destroy();
        }
    }

    private void Destroy()
    {
        IsCollected = true;
        QueueFree();
    }
}
