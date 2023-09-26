using Godot;
using System;

public class ParticlesManager : Particles2D
{
    private DateTime _spawnTime;

    public override void _Ready()
    {
        base._Ready();

        _spawnTime = DateTime.Now;
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        // Remove after particles are done
        if((DateTime.Now - _spawnTime).TotalSeconds > this.Lifetime)
            this.QueueFree();
    }
}
