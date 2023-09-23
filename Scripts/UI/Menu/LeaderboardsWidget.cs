using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class LeaderboardsWidget : VBoxContainer
{
    [Export] public Font Font;
    [Export] public int NumberOfResults = -1;

    public override void _Ready()
    {
        var data = LeaderboardsSave.Instance;
        IEnumerable<LeaderboardsSave.Entry> ordered = data.Entries.OrderByDescending(x => x.Score);
        if(NumberOfResults > 0)
            ordered = ordered.Take(NumberOfResults);

        var index = 0;
        var last = -1;
        foreach (var item in ordered)
        {
            if (last != item.Score)
                index++;

            var hBox = new HBoxContainer();
            hBox.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;

            var scoreLabel = new Label();
            var positionLabel = new Label();

            scoreLabel.Text = $"{item.Score}";
            scoreLabel.AddFontOverride("font", Font);
            scoreLabel.SizeFlagsHorizontal = (int)SizeFlags.ShrinkEnd;

            positionLabel.Text = last == item.Score ? "" : $"{index}.";
            positionLabel.AddFontOverride("font", Font);
            positionLabel.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;

            // 1st place
            if (index == 1)
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
    }
}
