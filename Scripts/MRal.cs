using Godot;
using System;
using MRalozerca2.Scripts;

public class MRal : Area2D
{
    [Signal]
    public delegate void OnColorChange(Color color);

    [Export] public float MoveSpeed = 1000;
    [Export] public float HorizontalPlayArea = 1920f / 2f;
    [Export] public NodePath FishCollectedPlayerPath;
    [Export] public PackedScene DeathParticlesScene;

    private AnimatedSprite _sprite;
    private AnimatedSprite _spriteScarf;
    private CollisionShape2D _collisionShape2D;
    private AudioStreamPlayer2D _fishCollectedPlayer;

    public const int MaxColors = 3;
    private GameColors _currentColor = 0;

    private float _currentMove = 0;

    private float _externalInput = 0;
    private void SetExternalInput(float input)
    {
        _externalInput = input;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if(DeathParticlesScene == null)
            throw new NullReferenceException("Death particles scene is null!");

        _sprite = GetNode<AnimatedSprite>("AnimatedSprite");
        _spriteScarf = _sprite.GetChild<AnimatedSprite>(0);

        _sprite.Frame = _spriteScarf.Frame = 0;
        _collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");

        _fishCollectedPlayer = GetNode<AudioStreamPlayer2D>(FishCollectedPlayerPath);

        this.GetManager().Connect(nameof(GameManager.OnGameOver), this, nameof(OnGameOver));
        this.GetManager().Connect(nameof(GameManager.OnGameStart), this, nameof(OnGameStart));

        EmitSignal(nameof(OnColorChange), _currentColor.ToColor());
    }

    void CheckCollisions()
    {
        var bodies = GetOverlappingAreas();
        foreach (var body in bodies)
        {
            if (!(body is Fish fish) || fish.Color != _currentColor) continue;
            this.GetManager().EmitSignal(nameof(GameManager.OnFishCollected), fish, true);
            _fishCollectedPlayer.Play();
        }
    }

    private void OnGameOver()
    {
        var deathParticles = DeathParticlesScene.Instance() as Particles2D;
        if(deathParticles == null)
            throw new NullReferenceException("Death particles is null or invalid type!");

        AddChild(deathParticles);

        var pMaterial = deathParticles.ProcessMaterial as ParticlesMaterial;
        if(pMaterial == null)
            throw new NullReferenceException("Particles material is null!");

        var newMaterial = pMaterial.Duplicate() as ParticlesMaterial;
        if(newMaterial == null)
            throw new NullReferenceException("New particles material is null!");

        newMaterial.InitialVelocity = MoveSpeed * 10f * this.GetManager().GameSpeedMultiplier;

        if (this._sprite.Scale.x < 0)
        {
            deathParticles.Scale = new Vector2(-deathParticles.Scale.x, deathParticles.Scale.y);
            var position = deathParticles.Position;
            if(!(_collisionShape2D.Shape is RectangleShape2D shape))
                throw new NullReferenceException("Collision shape is null or invalid type!");

            // When moving left and scaling x to negative, the particles will be spawned on the left side of the player
            // So we need to offset them to the right
            deathParticles.Position = new Vector2(position.x + shape.Extents.x * 2f, position.y);
        }

        if (_currentMove == 0)
        {
            newMaterial.Direction = new Vector3(0, 0, 0);
            newMaterial.Spread = 360f;
            newMaterial.InitialVelocity = newMaterial.InitialVelocity / 2f;
        }
        else
        {
            newMaterial.Direction = new Vector3(Math.Abs(_currentMove), 0, 0);
        }

        deathParticles.ProcessMaterial = newMaterial;

        deathParticles.Emitting = true;
        _sprite.Visible = false;
        _spriteScarf.Visible = false;
    }

    private void OnGameStart()
    {
        _sprite.Visible = true;
        _spriteScarf.Visible = true;
    }

    public GameColors NextColor()
    {
        _currentColor = _currentColor.NextColor();
        EmitSignal(nameof(OnColorChange), _currentColor.ToColor());
        return _currentColor;
    }

    public GameColors PreviousColor()
    {
        _currentColor = _currentColor.PreviousColor();
        EmitSignal(nameof(OnColorChange), _currentColor.ToColor());
        return _currentColor;
    }

    public override void _Process(float delta)
    {
        var manager = this.GetManager();
        if (!manager.IsGameRunning)
            return;

        CheckCollisions();

        base._Process(delta);
        var horizontal = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");

        // Ignore external input if keyboard input detected
        if (Mathf.Abs(horizontal) <= 0.01f && _externalInput != 0)
            horizontal = _externalInput;

        var lastPos = GlobalPosition;
        horizontal = Mathf.Clamp(horizontal, -1, 1);
        MoveLocalX(horizontal * delta * MoveSpeed * manager.GameSpeedMultiplier);

        _currentMove = horizontal;

        var scale = _sprite.Scale;
        if (horizontal > 0)
        {
            _sprite.Scale = new Vector2(Math.Abs(scale.x), scale.y);
        }
        else if (horizontal < 0)
        {
            _sprite.Scale = new Vector2(-Math.Abs(scale.x), scale.y);
        }

        if(!(_collisionShape2D.Shape is RectangleShape2D shape)) return;

        var horOffset = shape.Extents.x;
        if (GlobalPosition.x < -HorizontalPlayArea + horOffset)
        {
            GlobalPosition = new Vector2(-HorizontalPlayArea + horOffset, GlobalPosition.y);
        }
        else if (GlobalPosition.x > HorizontalPlayArea - horOffset)
        {
            GlobalPosition = new Vector2(HorizontalPlayArea - horOffset, GlobalPosition.y);
        }

        _sprite.Animation = _spriteScarf.Animation = lastPos != GlobalPosition ? "motion" : "idle";

        if(Input.IsActionJustPressed("next_color"))
            NextColor();
        else if(Input.IsActionJustPressed("prev_color"))
            PreviousColor();

        // Scarf color
        var color = _currentColor.ToColor();
        _spriteScarf.Modulate = color;
    }
}
