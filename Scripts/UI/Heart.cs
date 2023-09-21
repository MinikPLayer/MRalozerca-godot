using Godot;
using System;

public class Heart : TextureRect
{
    [Export] public Texture OnTexture;
    [Export] public Texture OffTexture;

    public void SetOn(bool on)
    {
        Texture = on ? OnTexture : OffTexture;
    }
}
