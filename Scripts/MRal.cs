using Godot;
using System;
using MRalozerca2.Scripts;

public class MRal : Area2D
{
    [Export] public float MoveSpeed = 1000;

    [Export] public float HorizontalPlayArea = 1920f / 2f;

    private AnimatedSprite _sprite;
    private AnimatedSprite _spriteScarf;
    private CollisionShape2D _collisionShape2D;

    public const int MaxColors = 3;
    GameColors _currentColor = GameColors.Green;

    private float _externalInput = 0;
    private void SetExternalInput(float input)
    {
        _externalInput = input;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _sprite = GetNode<AnimatedSprite>("AnimatedSprite");
        _spriteScarf = _sprite.GetChild<AnimatedSprite>(0);

        _sprite.Frame = _spriteScarf.Frame = 0;
        _collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
    }

    void CheckCollisions()
    {
        var bodies = GetOverlappingAreas();
        foreach (var body in bodies)
        {
            if (!(body is Fish fish) || fish.Color != _currentColor) continue;
            GameManager.FishCollected(fish, true);
        }
    }

    public void NextColor() => _currentColor = _currentColor.NextColor();
    public void PreviousColor() => _currentColor = _currentColor.PreviousColor();

    public override void _Process(float delta)
    {
        CheckCollisions();

        base._Process(delta);
        var horizontal = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");

        // Ignore external input if keyboard input detected
        if (Mathf.Abs(horizontal) <= 0.01f && _externalInput != 0)
            horizontal = _externalInput;

        var lastPos = GlobalPosition;
        horizontal = Mathf.Clamp(horizontal, -1, 1);
        MoveLocalX(horizontal * delta * MoveSpeed * GameManager.GameSpeedMultiplier);

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
