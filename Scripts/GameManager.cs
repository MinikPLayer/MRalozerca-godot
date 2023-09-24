using Godot;
using System;
using System.ComponentModel;

public static class GameManagerExtensions
{
    public static GameManager GetManager(this Node n)
    {
        var manager = n.GetNode<GameManager>("/root/Game");
        #if DEBUG
            if (manager == null)
                throw new NullReferenceException("GameManager not found! (Forgot to add?)");
        #endif

        return manager;
    }
}

// Planned modes:
// [WIP] Endless / Classic - Collect as many fish as you can, you gain a life for every 10 fish collected, you lose a life for every fish lost, fish spawn faster and faster
// [TODO] Endless+ - Just like classic, but instead of speeding up every fish, you speed up every level up. You level up when experience bar is full, you gain experience for every fish collected, you lose experience for every fish lost
// [TODO] Time Attack - Collect as many fish as you can in a given time
public class GameManager : Node2D
{
    public float GameSpeedMultiplier { get; private set; } = 1.0f;

    public bool IsGameStarted { get; set; } = false;
    public bool IsGamePaused { get; set; } = false;
    public bool IsGameOver { get; set; } = true;
    public bool IsGameRunning => !IsGamePaused && !IsGameOver && IsGameStarted;


    [Signal]
    public delegate void OnFishCollected(Fish fish, bool collected);

    [Signal]
    public delegate void OnGameStart();

    [Signal]
    public delegate void OnGameOver();

    [Signal]
    public delegate void OnGamePaused();

    [Signal]
    public delegate void OnGameResumed();

    [Signal]
    public delegate void OnLivesChanged(int newLives, int maxLives);

    private int _currentLives = 0;
    public int CurrentLives
    {
        get => _currentLives;
        set
        {
            _currentLives = Mathf.Clamp(value, 0, this.GetDifficulty().MaxLives);
            EmitSignal(nameof(OnLivesChanged), _currentLives, this.GetDifficulty().MaxLives);

            if (_currentLives == 0)
                EmitSignal(nameof(OnGameOver));

        }
    }

    private void FishCollected(Fish f, bool collected)
    {
        if (collected)
        {
            GameSpeedMultiplier += GameSpeedMultiplier * this.GetDifficulty().GameSpeedIncreaseMultiplier;
        }
        else
        {
            GameSpeedMultiplier = this.GetDifficulty().GameSpeedMultiplierStart + (GameSpeedMultiplier - this.GetDifficulty().GameSpeedMultiplierStart) * this.GetDifficulty().GameSpeedDecreaseMultiplier;
            CurrentLives--;
        }

        GD.Print($"Game speed: {GameSpeedMultiplier}");
        GameSpeedMultiplier = Mathf.Clamp(GameSpeedMultiplier, this.GetDifficulty().GameSpeedMultiplierStart, this.GetDifficulty().GameSpeedMax);
    }

    private void OnGameStartedEvent()
    {
        IsGameOver = false;
        IsGameStarted = true;
        CurrentLives = this.GetDifficulty().StartLives;
        GameSpeedMultiplier = this.GetDifficulty().GameSpeedMultiplierStart;
    }

    private void OnGameOverEvent()
    {
        IsGameOver = true;
    }

    private void OnGamePausedEvent()
    {
        IsGamePaused = true;
    }

    private void OnGameResumedEvent()
    {
        IsGamePaused = false;
    }

    public override void _Ready()
    {
        base._Ready();

        Connect(nameof(OnFishCollected), this, nameof(FishCollected));
        Connect(nameof(OnGameOver), this, nameof(OnGameOverEvent));
        Connect(nameof(OnGameStart), this, nameof(OnGameStartedEvent));
        Connect(nameof(OnGamePaused), this, nameof(OnGamePausedEvent));
        Connect(nameof(OnGameResumed), this, nameof(OnGameResumedEvent));

        CurrentLives = this.GetDifficulty().StartLives;
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (Input.IsActionJustPressed("toggle_fullscreen"))
            OS.WindowFullscreen = !OS.WindowFullscreen;
    }
}
