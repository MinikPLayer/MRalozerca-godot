using Godot;
using System;

public class GameOverUiManager : Popup
{
    [Export] public PackedScene GameOverUiScene;

    public override void _Ready()
    {
        base._Ready();

        this.GetManager().Connect(nameof(GameManager.OnGameOver), this, nameof(OnGameOver));
        this.GetManager().Connect(nameof(GameManager.OnGameStart), this, nameof(OnGameStart));
    }

    private void OnGameOver()
    {
        this.Show();
    }

    private void OnGameStart()
    {
        this.Hide();
    }
}
