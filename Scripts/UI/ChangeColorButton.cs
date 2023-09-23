using Godot;
using System;
using MRalozerca2.Scripts;

public class ChangeColorButton : TouchScreenButton
{
    [Export] public NodePath LeftButtonPath;
    [Export] public NodePath RightButtonPath;
    [Export] public NodePath IndicatorPath;
    [Export] public NodePath MRalPath;

    [Export] public float PressedOpacity = 1.0f;
    [Export] public float NotPressedOpacity = 0.5f;

    [Export] public float ButtonsSeparationDeadzone = 200.0f;

    private TextureRect _indicator;
    private TouchScreenButton _leftButton;
    private TouchScreenButton _rightButton;
    // ReSharper disable once IdentifierTypo
    private MRal _mral;

    private Color _currentColor = Colors.White;

    public override void _Ready()
    {
        base._Ready();

        _leftButton = GetNode<TouchScreenButton>(LeftButtonPath);
        _rightButton = GetNode<TouchScreenButton>(RightButtonPath);
        _indicator = GetNode<TextureRect>(IndicatorPath);
        _mral = GetNode<MRal>(MRalPath);
    }

    public void OnColorChange()
    {
        GD.Print("Color changed!");
        var color = _mral.NextColor();

        _currentColor = color.ToColor();
        Input.VibrateHandheld(10);
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        // Resize this button to fit the screen between the left and right buttons
        // TODO: Do this only when the screen size changes
        var viewport = GetViewport();
        var leftButtonWidth = 200 * _leftButton.Scale.x;
        var rightButtonRect = 200 * _rightButton.Scale.x;

        var left = _leftButton.GlobalPosition.x + leftButtonWidth + ButtonsSeparationDeadzone;
        var right = viewport.Size.x - rightButtonRect - ButtonsSeparationDeadzone;
        var width = right - left;
        var newRect = new Rect2(left, 0, width, viewport.Size.y);
        GlobalPosition = newRect.Position;
        Scale = newRect.Size;

        _indicator.Modulate = new Color(_currentColor.r, _currentColor.g, _currentColor.b, IsPressed() ? PressedOpacity : NotPressedOpacity);
    }
}
