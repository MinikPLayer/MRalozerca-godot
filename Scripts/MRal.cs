using Godot;
using System;

public class MRal : Area2D
{
    [Export] public float MoveSpeed = 1000;

    [Export] public float HorizontalPlayArea = 1920f / 2f;

    private AnimatedSprite _sprite;
    private CollisionShape2D _collisionShape2D;

    public const int MaxColors = 3;
    public int CurrentColor = 0;

    private float _externalInput = 0;
    private void SetExternalInput(float input)
    {
        _externalInput = input;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _sprite = GetNode<AnimatedSprite>("AnimatedSprite");
        _collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
    }

    void CheckCollisions()
    {
        var bodies = GetOverlappingAreas();
        foreach (var body in bodies)
        {
            if (!(body is Fish fish)) continue;
            GameManager.FishCollected(fish, true);
        }
    }

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
        MoveLocalX(horizontal * delta * MoveSpeed);

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

        _sprite.Animation = lastPos != GlobalPosition ? "motion" : "idle";
    }
}
