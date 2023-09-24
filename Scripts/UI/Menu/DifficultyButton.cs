using Godot;
using System;

public class DifficultyButton : TextureRect
{
    [Export] public Texture[] Textures;

    [Export] public NodePath DifficultyLabelPath;

    [Signal]
    public delegate void OnDifficultyChanged(int newLevel);

    private Label _difficultyLevelLabel;

    private void SetDifficultyIndex(int index)
    {
        Config.Instance.DifficultyLevel = index;
        Difficulty.SetDifficulty(index);
    }

    private void UpdateDifficulty()
    {
        var index = Difficulty.CurrentLevelIndex;

        var texture = Textures[index];
        var name = Difficulty.Levels[index].name;
        var color = Difficulty.Levels[index].color;

        _difficultyLevelLabel.Text = name;
        _difficultyLevelLabel.AddColorOverride("font_color", color);
        Texture = texture;
        Modulate = color;
    }

    private void _on_DifficultyButton_pressed()
    {
        var index = Difficulty.CurrentLevelIndex;
        index++;
        if(index >= Difficulty.Levels.Keys.Count)
            index = 0;

        SetDifficultyIndex(index);
        UpdateDifficulty();
        Config.Instance.Save();

        EmitSignal(nameof(OnDifficultyChanged), index);
    }

    public override void _Ready()
    {
        base._Ready();
        SetDifficultyIndex(Config.Instance.DifficultyLevel);

        if(Textures.Length != Difficulty.Levels.Count)
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
