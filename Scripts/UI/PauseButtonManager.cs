using Godot;
using System;

public class PauseButtonManager : Control
{
    public override void _Ready()
    {
        this.Hide();
        this.GetManager().Connect(nameof(GameManager.OnGameStart), this, nameof(OnGameStarted));
    }

    private void OnGameStarted()
    {
        this.Show();
    }
}
