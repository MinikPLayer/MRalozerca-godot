using Godot;
using System;
using System.Collections.Generic;

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

        public static bool operator==(Level a, Level b)
        {
            return a.MaxColors == b.MaxColors &&
                   a.GameSpeedMultiplierStart == b.GameSpeedMultiplierStart &&
                   a.GameSpeedIncreaseMultiplier == b.GameSpeedIncreaseMultiplier &&
                   a.GameSpeedDecreaseMultiplier == b.GameSpeedDecreaseMultiplier &&
                   a.GameSpeedMax == b.GameSpeedMax &&
                   a.StartLives == b.StartLives &&
                   a.MaxLives == b.MaxLives;
        }

        public static bool operator !=(Level a, Level b)
        {
            return !(a == b);
        }
    }

    public static readonly Level LevelEasy = new Level(
        maxColors: 2,
        gameSpeedMultiplierStart: 1.0f,
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

    public static Dictionary<int, (string name, Color color, Level levelData)> Levels { get; } = new Dictionary<int, (string, Color, Level)>()
    {
        { 0, ("Easy", new Color("00FF93"),  LevelEasy) },
        { 1, ("Normal", new Color("00FF15"), LevelNormal) },
        { 2, ("Hard", new Color("DAFF00"), LevelHard) },
        { 3, ("Very Hard", new Color("FF7D00"), LevelVeryHard) },
        { 4, ("Hardcore", new Color("FF0000"), LevelHardcore) },
    };

    public static int CurrentLevelIndex { get; private set; } = 1;
    public static Level CurrentLevel => Levels[CurrentLevelIndex].levelData;

    public static void SetDifficulty(int level)
    {
        CurrentLevelIndex = level;
    }

    public static Level GetDifficulty(this Node node) => CurrentLevel;

}
