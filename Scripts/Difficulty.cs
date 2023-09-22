using Godot;
using System;

public static class Difficulty
{
    public struct Level
    {
        public int MaxColors { get; set; }
        public float GameSpeedMultiplierStart { get; set; }
        public float GameSpeedIncreaseMultiplier { get; set; }
        public float GameSpeedDecreaseMultiplier { get; set; }
        public float GameSpeedMax { get; set; }
        public int StartLives { get; set; }
        public int MaxLives { get; set; }

        public Level(int maxColors, float gameSpeedMultiplierStart, float gameSpeedIncreaseMultiplier, float gameSpeedDecreaseMultiplier, float gameSpeedMax, int startLives, int maxLives)
        {
            MaxColors = maxColors;
            GameSpeedMultiplierStart = gameSpeedMultiplierStart;
            GameSpeedIncreaseMultiplier = gameSpeedIncreaseMultiplier;
            GameSpeedDecreaseMultiplier = gameSpeedDecreaseMultiplier;
            GameSpeedMax = gameSpeedMax;
            StartLives = startLives;
            MaxLives = maxLives;
        }
    }

    public static readonly Level LevelEasy = new Level(
        maxColors: 2,
        gameSpeedMultiplierStart: 0.5f,
        gameSpeedIncreaseMultiplier: 0.02f,
        gameSpeedDecreaseMultiplier: 0.75f,
        gameSpeedMax: 1.5f,
        startLives: 5,
        maxLives: 5
    );

    public static readonly Level LevelNormal = new Level(
        maxColors: 3,
        gameSpeedMultiplierStart: 1.0f,
        gameSpeedIncreaseMultiplier: 0.03f,
        gameSpeedDecreaseMultiplier: 0.5f,
        gameSpeedMax: 2.0f,
        startLives: 3,
        maxLives: 5
    );

    public static readonly Level LevelHard = new Level(
        maxColors: 4,
        gameSpeedMultiplierStart: 1.0f,
        gameSpeedIncreaseMultiplier: 0.04f,
        gameSpeedDecreaseMultiplier: 0.4f,
        gameSpeedMax: 2.0f,
        startLives: 3,
        maxLives: 3
    );

    public static readonly Level LevelVeryHard = new Level(
        maxColors: 5,
        gameSpeedMultiplierStart: 1.2f,
        gameSpeedIncreaseMultiplier: 0.05f,
        gameSpeedDecreaseMultiplier: 0.3f,
        gameSpeedMax: 3.0f,
        startLives: 2,
        maxLives: 3
    );

    public static readonly Level LevelHardcore = new Level(
        maxColors: 5,
        gameSpeedMultiplierStart: 1.2f,
        gameSpeedIncreaseMultiplier: 0.05f,
        gameSpeedDecreaseMultiplier: 0.3f,
        gameSpeedMax: 3.0f,
        startLives: 1,
        maxLives: 1
    );

    public static Level CurrentLevel { get; private set; } = LevelHardcore;

    public static Level GetDifficulty(this Node node) => CurrentLevel;

}
