using Godot;
using System;

public class FishDestroyArea : Area2D
{
    public void OnAreaEntered(Area2D body)
    {
        if (body is Fish f && !f.IsCollected)
        {
            GD.Print("Fish lost!");
            this.GetManager().EmitSignal(nameof(GameManager.OnFishCollected), f, false);
        }
    }
}
