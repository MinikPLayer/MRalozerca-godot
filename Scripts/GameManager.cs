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
    [Export]
    public float MaxGameSpeed { get; set; } = 2.0f;
    [Export]
    public float GameSpeedIncreaseStep { get; set; } = 0.1f;
    [Export]
    public float GameSpeedDecreaseStep { get; set; } = 0.05f;
    [Export]
    public int StartLives { get; set; } = 3;

    [Export] public int MaxLives { get; set; } = 3;

    public float GameSpeedMultiplier { get; private set; } = 1.0f;
    // TODO: Add menu to start the game
    public bool IsGameRunning { get; private set; } = false;


    [Signal]
    public delegate void OnFishCollected(Fish fish, bool collected);

    [Signal]
    public delegate void OnGameStart();

    [Signal]
    public delegate void OnGameOver();

    [Signal]
    public delegate void OnLivesChanged(int newLives, int maxLives);

    private int _currentLives = 0;
    public int CurrentLives
    {
        get => _currentLives;
        set
        {
            _currentLives = Mathf.Clamp(value, 0, MaxLives);
            EmitSignal(nameof(OnLivesChanged), _currentLives, MaxLives);

            if (_currentLives == 0)
                EmitSignal(nameof(OnGameOver));

        }
    }

    private void FishCollected(Fish f, bool collected)
    {
        if (collected)
        {
            GameSpeedMultiplier += GameSpeedIncreaseStep;
        }
        else
        {
            GameSpeedMultiplier -= GameSpeedDecreaseStep;
            CurrentLives--;
        }

        GameSpeedMultiplier = Mathf.Clamp(GameSpeedMultiplier, 1.0f, MaxGameSpeed);
    }

    private void StartGame()
    {
        IsGameRunning = true;
        CurrentLives = StartLives;
        GameSpeedMultiplier = 1.0f;
    }

    private void GameOver()
    {
        IsGameRunning = false;
    }

    public override void _Ready()
    {
        base._Ready();

        Connect(nameof(OnFishCollected), this, nameof(FishCollected));
        Connect(nameof(OnGameOver), this, nameof(GameOver));
        Connect(nameof(OnGameStart), this, nameof(StartGame));

        // TODO: Add menu to start the game
        EmitSignal(nameof(OnGameStart));
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (Input.IsActionJustPressed("toggle_fullscreen"))
            OS.WindowFullscreen = !OS.WindowFullscreen;
    }
}
