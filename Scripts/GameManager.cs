using Godot;
using System;

public static class GameManager
{
    public static float MaxGameSpeed { get; set; } = 2.0f;
    public static float GameSpeedIncreaseStep { get; set; } = 0.1f;
    public static float GameSpeedDecreaseStep { get; set; } = GameSpeedIncreaseStep * 0.5f;

    public static float GameSpeedMultiplier { get; private set; } = 1.0f;

    public delegate void FishCollectedHandler(Fish fish, bool collected);
    public static event FishCollectedHandler OnFishCollected = delegate { };

    public static void FishCollected(Fish f, bool collected)
    {
        if (collected)
        {
            GameSpeedMultiplier += GameSpeedIncreaseStep;
        }
        else
        {
            GameSpeedMultiplier -= GameSpeedDecreaseStep;
        }

        GameSpeedMultiplier = Mathf.Clamp(GameSpeedMultiplier, 1.0f, MaxGameSpeed);
        OnFishCollected(f, collected);
    }

    // TODO: Add menu to start the game
    public static bool IsGameRunning { get; private set; } = true;

    public delegate void GameStartHandler();
    public static event GameStartHandler OnGameStart = delegate { };

    public static void StartGame()
    {
        IsGameRunning = true;
        GameSpeedMultiplier = 1.0f;
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
