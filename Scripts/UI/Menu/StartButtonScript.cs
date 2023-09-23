using Godot;
using System;

public class StartButtonScript : Button
{
    private void Disable(Node n)
    {
        if (n is Button b)
            b.Disabled = true;

        foreach (Node child in n.GetChildren())
            Disable(child);
    }

    public void OnPressed()
    {
        // Disable all buttons in current scene
        Disable(GetTree().Root);
    }
}
