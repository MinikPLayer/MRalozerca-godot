using Godot;
using System;

public class MenuTransition : Node2D
{
    [Export] public PackedScene GameScene;
    [Export] public NodePath GameUiNodePath;

    [Export] public bool IsReceiver = false;

    [Export] public float TransitionHeight = 2080f;
    [Export] public float TransitionSpeed = 7500f;

    private bool _isTransitioning = false;
    private Control _gameUiNode;

    public void TransitionToGame()
    {
        // GetTree().ChangeSceneTo(GameScene);
        _isTransitioning = true;
    }

    public override void _Ready()
    {
        base._Ready();
        _gameUiNode = GetNode<Control>(GameUiNodePath);

        if (IsReceiver)
        {
            GlobalPosition = new Vector2(0, -TransitionHeight);
            _isTransitioning = true;
        }
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (_isTransitioning)
        {
            _gameUiNode.Show();
            var position = GlobalPosition;
            var diff = Mathf.Clamp(1.0f - Mathf.Abs((Mathf.Abs(position.y) - TransitionHeight) / TransitionHeight), 0.001f, 1.0f);
            _gameUiNode.Modulate = new Color(1, 1, 1, 1.0f - diff);

            // Speed up transition if we are NOT the receiver
            // This is to make the transition faster / more responsive for the player
            if (!IsReceiver)
                diff += 0.05f;

            if (IsReceiver)
            {
                if (position.y < 0)
                {
                    position.y += TransitionSpeed * delta * diff;
                    GlobalPosition = position;
                    _gameUiNode.RectPosition = new Vector2(0, -position.y);
                }
                else
                {
                    _isTransitioning = false;
                    GlobalPosition = new Vector2(0, 0);
                    this.GetManager().EmitSignal(nameof(GameManager.OnGameStart));
                }
            }
            else
            {
                if(position.y < -TransitionHeight)
                {
                    GetTree().ChangeSceneTo(GameScene);
                }
                else
                {
                    position.y -= TransitionSpeed * delta * diff;
                    GlobalPosition = position;
                    _gameUiNode.RectPosition = new Vector2(0, -position.y);
                }
            }
        }
    }
}
