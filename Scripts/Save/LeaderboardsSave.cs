using Godot;
using System;
using System.Collections.Generic;

public class LeaderboardsSave : Node
{
    public struct Entry
    {
        public DateTime Date { get; set; }
        public int Score { get; set; }

        public Entry(DateTime date, int score)
        {
            Date = date;
            Score = score;
        }
    }

    public List<Entry> Entries { get; set; } = new List<Entry>
    {
        new Entry(DateTime.Now, 123),
        new Entry(new DateTime(2137, 11, 11), 2137),
        new Entry(new DateTime(2137, 11, 11), 2137),
        new Entry(new DateTime(2137, 11, 11), 2137),
        new Entry(new DateTime(2137, 11, 11), 213127),
        new Entry(new DateTime(2137, 11, 11), 22137),
        new Entry(new DateTime(2137, 11, 11), 234137),
        new Entry(new DateTime(2137, 11, 11), 213127),
        new Entry(new DateTime(2137, 11, 11), 213721),
        new Entry(new DateTime(2137, 11, 11), 21375),
        new Entry(new DateTime(2137, 11, 11), 213712),
        new Entry(new DateTime(2137, 11, 11), 2137231),
        new Entry(new DateTime(2137, 11, 11), 213755),
        new Entry(new DateTime(2137, 11, 11), 2137235),
        new Entry(new DateTime(2137, 11, 11), 2137235),
        new Entry(new DateTime(2137, 11, 11), 2132677),
        new Entry(new DateTime(2137, 11, 11), 32452137),
        new Entry(new DateTime(2137, 11, 11), 2132237),
        new Entry(new DateTime(2137, 11, 11), 213772),
        new Entry(new DateTime(2137, 11, 11), 2137),
        new Entry(new DateTime(2137, 11, 11), 2137),
        new Entry(new DateTime(2137, 11, 11), 2137),
        new Entry(new DateTime(2137, 11, 11), 2137),
        new Entry(new DateTime(2137, 11, 11), 2137),
        new Entry(new DateTime(2137, 11, 11), 2137),
        new Entry(new DateTime(2137, 11, 11), 2137),
        new Entry(new DateTime(2137, 11, 11), 2137),
        new Entry(new DateTime(2137, 11, 11), 2137),
        new Entry(new DateTime(2137, 11, 11), 2137),
        new Entry(new DateTime(2137, 11, 11), 2137),
    };

    public static LeaderboardsSave Instance { get; private set; } = Load() ?? new LeaderboardsSave();

    private static string GetDefaultPath() => "user://leaderboards.sav";
    private const string Password = "very_strong_password_yeah_this_is_fine_just_a_mock_xd";

    public void Save(string path = null)
    {
        if(path == null)
            path = GetDefaultPath();

        var file = new File();
        // file.OpenEncryptedWithPass(path, File.ModeFlags.Write, Password);
        file.Open(path, File.ModeFlags.Write);

        file.StoreLine(Entries.Count.ToString());
        foreach(var entry in Entries)
        {
            file.StoreLine(entry.Date.Ticks.ToString());
            file.StoreLine(entry.Score.ToString());
            file.StoreLine("#EntryEnd#");
        }

        file.Close();
    }

    public static LeaderboardsSave Load(string path = null)
    {
        if(path == null)
            path = GetDefaultPath();

        var file = new File();
        var ret = file.Open(path, File.ModeFlags.Read);
        if (ret != Error.Ok)
        {
            GD.PrintErr($"Failed to open file {path}! Error: {ret}");
            return null;
        }

        var count = int.Parse(file.GetLine());
        var entries = new List<Entry>(count);
        for(var i = 0; i < count; i++)
        {
            var date = new DateTime(long.Parse(file.GetLine()));
            var score = int.Parse(file.GetLine());
            var end = file.GetLine();
            if(end != "#EntryEnd#")
            {
                GD.PrintErr($"Failed to load file {path}! Error: Invalid entry end");
                return null;
            }
            entries.Add(new Entry { Date = date, Score = score });
        }

        file.Close();
        return new LeaderboardsSave() { Entries = entries };
    }
}
