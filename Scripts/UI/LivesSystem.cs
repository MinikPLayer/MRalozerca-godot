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

        var heartWidth = 0;
        // Add new children
        for (var i = 0; i < maxLives; i++)
        {
            var live = LiveScenePath.Instance<Heart>();
            heartWidth = live.Texture.GetWidth();
            live.SetOn(i < lives);
            AddChild(live);
        }

        var width = (maxLives - 1) * (heartWidth + SeparatorWidth) + heartWidth;
        MarginLeft = -((width - SeparatorWidth) * this.RectScale.x) / 2f;
        SetSize(new Vector2(width, RectSize.y), true);
    }
}
