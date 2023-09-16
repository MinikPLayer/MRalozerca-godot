using Godot;
using System;

public class FishDestroyArea : Area2D
{
    public void OnAreaEntered(Area2D body)
    {
        if (body is Fish f)
        {
            GD.Print("Fish lost!");
            GameManager.FishCollected(f, false);
        }
    }
}
