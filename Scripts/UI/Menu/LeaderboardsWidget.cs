using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class LeaderboardsWidget : VBoxContainer
{
	[Export] public NodePath DifficultyLabelPath;

	[Export] public Font Font;
	[Export] public int NumberOfResults = -1;

	private Label _difficultyLabel;

	public void UpdateLeaderboards(int difficultyLevel)
	{
		foreach(var c in GetChildren())
			(c as Node)?.QueueFree();

		var data = LeaderboardsSave.Instance;
		IEnumerable<LeaderboardsSave.Entry> ordered = data.Entries.Where(x => x.DifficultyLevel == difficultyLevel).OrderByDescending(x => x.Score);
		if(NumberOfResults > 0)
			ordered = ordered.Take(NumberOfResults);

		var index = -1;
		var last = -2;

		var emptyList = new List<LeaderboardsSave.Entry>();
		emptyList.Add(new LeaderboardsSave.Entry(DateTime.MinValue, -1, Difficulty.CurrentLevelIndex));

		ordered = emptyList.Concat(ordered);

		foreach (var item in ordered)
		{
			if (last != item.Score)
				index++;

			var score = index == 0 ? "Score" : $"{item.Score}";
			var indexText = index == 0 ? "Place" : $"{index}.";

			var hBox = new HBoxContainer();
			hBox.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;

			var scoreLabel = new Label();
			var positionLabel = new Label();

			scoreLabel.Text = $"{score}";
			scoreLabel.AddFontOverride("font", Font);
			scoreLabel.SizeFlagsHorizontal = (int)SizeFlags.ShrinkEnd;

			positionLabel.Text = last == item.Score ? "" : $"{indexText}";
			positionLabel.AddFontOverride("font", Font);
			positionLabel.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;

			// Legend
			if (index == 0)
			{
				scoreLabel.Modulate = positionLabel.Modulate = Colors.DarkGray;
			}
			else if (index == 1) // 1st place
			{
				scoreLabel.Modulate = positionLabel.Modulate = Colors.Gold;
			}
			else if (index == 2)
			{
				scoreLabel.Modulate = positionLabel.Modulate = Colors.Silver;
			}
			else if (index == 3)
			{
				scoreLabel.Modulate = positionLabel.Modulate = Colors.SaddleBrown;
			}

			hBox.AddChild(positionLabel);
			hBox.AddChild(scoreLabel);

			AddChild(hBox);

			last = item.Score;
		}

		_difficultyLabel.Text = Difficulty.Levels[difficultyLevel].name;
		_difficultyLabel.AddColorOverride("font_color", Difficulty.Levels[difficultyLevel].color);
	}

	public override void _Ready()
	{
		_difficultyLabel = GetNode<Label>(DifficultyLabelPath);

		UpdateLeaderboards(Difficulty.CurrentLevelIndex);
	}
}
