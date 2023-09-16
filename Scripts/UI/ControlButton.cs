using Godot;
using System;

public class ControlButton : TouchScreenButton
{
    [Export]
    public NodePath SymbolPath;

    [Export] public float PressedOpacity = 1.0f;
    [Export] public float NotPressedOpacity = 0.2f;

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        var pressedColor = new Color(1, 1, 1, PressedOpacity);
        var notPressedColor = new Color(1, 1, 1, NotPressedOpacity);
        var color = IsPressed() ? pressedColor : notPressedColor;

        if (!OS.HasTouchscreenUiHint())
            color = Colors.Transparent;

        Modulate = color;

        if (SymbolPath.IsEmpty()) return;

        var targetNode = GetNode<TextureRect>(SymbolPath);
        targetNode.Modulate = Modulate;
    }
}
