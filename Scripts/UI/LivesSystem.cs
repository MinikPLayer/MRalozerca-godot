using Godot;
using System;

public class LivesSystem : HBoxContainer
{
    [Export] public PackedScene LiveScenePath;
    [Export] public int SeparatorWidth = 50;

    // Don't forget to connect this signal in the editor!
    private void OnLivesChanged(int lives, int maxLives)
    {
        // Remove all children
        foreach (Node child in GetChildren())
            child.Free();

        var width = 0f;
        // Add new children
        for (var i = 0; i < maxLives; i++)
        {
            var live = LiveScenePath.Instance<Heart>();
            width += live.Texture.GetWidth() + SeparatorWidth;
            live.SetOn(i < lives);
            AddChild(live);
        }

        MarginLeft = -width * this.RectScale.x;
        SetSize(new Vector2(width, 500), true);
    }
}
