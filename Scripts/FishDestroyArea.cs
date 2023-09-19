using Godot;
using System;

public class FishDestroyArea : Area2D
{
    [Export] public NodePath FishLostPlayerPath;
    private AudioStreamPlayer2D _fishLostPlayer;

    public override void _Ready()
    {
        base._Ready();
        _fishLostPlayer = GetNode<AudioStreamPlayer2D>(FishLostPlayerPath);
    }

    public void OnAreaEntered(Area2D body)
    {
        if (body is Fish f && !f.IsCollected)
        {
            GD.Print("Fish lost!");
            this.GetManager().EmitSignal(nameof(GameManager.OnFishCollected), f, false);
            _fishLostPlayer.Play();
        }
    }
}
