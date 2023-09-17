using Godot;
using System;

public class PointsSystem : Label
{
    [Export] public int[] PointsPerFish = new int[]
    {
        100, 200, 500, 1000, 2000, 5000
    };

    [Export] public int PointPerLife = 100_000;

    private int _lastLifeUp = 0;
    private int _currentPoints = 0;
    public int CurrentPoints
    {
        get => _currentPoints;
        set
        {
            _currentPoints = value;
            Text = "Points: " + _currentPoints.ToString();
        }
    }
    private int _currentCombo = 0;

    public override void _Ready()
    {
        var manager = this.GetManager();
        manager.Connect(nameof(GameManager.OnFishCollected), this, nameof(OnFishCollected));
        manager.Connect(nameof(GameManager.OnGameStart), this, nameof(OnGameStart));
    }

    private void OnGameStart()
    {
        _currentCombo = 0;
        CurrentPoints = 0;
    }

    private void OnFishCollected(Fish f, bool collected)
    {
        if (!collected)
        {
            _currentCombo = 0;
            return;
        }

        var points = PointsPerFish[_currentCombo];
        CurrentPoints += points;
        _currentCombo = Mathf.Clamp(_currentCombo + 1, 0, PointsPerFish.Length - 1);

        if(CurrentPoints - _lastLifeUp >= PointPerLife)
        {
            var newLives = (CurrentPoints - _lastLifeUp) / PointPerLife;
            _lastLifeUp += newLives * PointPerLife;
            this.GetManager().CurrentLives += newLives;
        }
    }

}
