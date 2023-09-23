using Godot;
using System;

public class DifficultyButton : TextureRect
{

    [Export] public Texture[] Textures;
    [Export] public string[] DifficultyNames;
    [Export] public Color[] Colors;

    [Export] public NodePath DifficultyLabelPath;

    private Label _difficultyLevelLabel;

    private readonly Difficulty.Level[] _levels = new Difficulty.Level[]
    {
        Difficulty.LevelEasy,
        Difficulty.LevelNormal,
        Difficulty.LevelHard,
        Difficulty.LevelVeryHard,
        Difficulty.LevelHardcore
    };

    private int GetDifficultyIndex()
    {
        var curDif = Difficulty.CurrentLevel;
        for(var i = 0; i < _levels.Length; i++)
        {
            if(curDif == _levels[i])
            {
                return i;
            }
        }

        throw new Exception("DifficultyButton: Current difficulty not found");
    }

    private void SetDifficultyIndex(int index)
    {
        Difficulty.SetDifficulty(_levels[index]);
    }

    private void UpdateDifficulty()
    {
        var index = GetDifficultyIndex();

        var texture = Textures[index];
        var name = DifficultyNames[index];
        var color = Colors[index];

        _difficultyLevelLabel.Text = name;
        _difficultyLevelLabel.AddColorOverride("font_color", color);
        Texture = texture;
        Modulate = color;
    }

    private void _on_DifficultyButton_pressed()
    {
        var index = GetDifficultyIndex();
        index++;
        if(index >= _levels.Length)
        {
            index = 0;
        }

        SetDifficultyIndex(index);
        UpdateDifficulty();
    }

    public override void _Ready()
    {
        base._Ready();

        if(Textures.Length != DifficultyNames.Length || Textures.Length != Colors.Length)
        {
            GD.PrintErr("DifficultyButton: Textures, DifficultyNames and Colors must have the same length");
            throw new Exception("DifficultyButton: Textures, DifficultyNames and Colors must have the same length");
        }

        _difficultyLevelLabel = GetNode<Label>(DifficultyLabelPath);
        if(_difficultyLevelLabel == null)
        {
            GD.PrintErr("DifficultyButton: Label not found");
            throw new Exception("DifficultyButton: Label not found");
        }

        UpdateDifficulty();
    }
}
