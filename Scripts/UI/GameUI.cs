using Godot;
using System;

public class GameUI : Control
{
    [Export] public NodePath LeftButtonPath;
    [Export] public NodePath RightButtonPath;

    private TouchScreenButton _leftButton;
    private TouchScreenButton _rightButton;

    private float _lastHorizontal = 0;

    [Signal]
    public delegate void InputChangedSignal(float newInput);

    public override void _Ready()
    {
        _leftButton = GetNode<TouchScreenButton>(LeftButtonPath);
        _rightButton = GetNode<TouchScreenButton>(RightButtonPath);
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        var horizontal = 0;
        if (_leftButton.IsPressed())
            horizontal -= 1;

        if (_rightButton.IsPressed())
            horizontal += 1;

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (horizontal != _lastHorizontal)
        {
            EmitSignal("InputChangedSignal", horizontal);
            _lastHorizontal = horizontal;
        }
    }
}
