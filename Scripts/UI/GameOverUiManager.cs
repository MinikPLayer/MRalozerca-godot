using Godot;
using System;

public class GameOverUiManager : Node
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
        var gameOverUi = GameOverUiScene.Instance();
        AddChild(gameOverUi);
    }

    private void OnGameStart()
    {
        foreach(var c in GetChildren())
            (c as Node)?.QueueFree();
    }
}
