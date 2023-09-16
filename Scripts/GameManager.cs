using Godot;
using System;

public static class GameManager
{
    public delegate void FishCollectedHandler(Fish fish, bool collected);

    public static event FishCollectedHandler OnFishCollected = delegate { };
    public static void FishCollected(Fish f, bool collected) => OnFishCollected(f, collected);

    // TODO: Add menu to start the game
    public static bool IsGameRunning { get; private set; } = true;

    public delegate void GameStartHandler();
    public static event GameStartHandler OnGameStart = delegate { };

    public static void StartGame()
    {
        IsGameRunning = true;
        OnGameStart();
    }

    public delegate void GameOverHandler();
    public static event GameOverHandler OnGameOver = delegate { };

    public static void GameOver()
    {
        IsGameRunning = false;
        OnGameOver();
    }
}
