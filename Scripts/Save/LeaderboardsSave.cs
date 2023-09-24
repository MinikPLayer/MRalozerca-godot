using Godot;
using System;
using System.Collections.Generic;

public class LeaderboardsSave : Node
{
    public struct Entry
    {
        public DateTime Date { get; set; }
        public int Score { get; set; }
        public int DifficultyLevel { get; set; }

        public Entry(DateTime date, int score, int difficultyLevel)
        {
            Date = date;
            Score = score;
            DifficultyLevel = difficultyLevel;
        }
    }

    public List<Entry> Entries { get; set; } = new List<Entry>();
    public bool SaveLock { get; private set; } = false;

    public void UnlockSave()
    {
        if (SaveLock)
        {
            SaveLock = false;
            Save();
        }
    }

    public void AddEntry(int score, int difficulty, bool save = true)
    {
        var entry = new Entry(DateTime.Now, score, difficulty);
        Entries.Add(entry);

        if (save)
            Save();
    }

    public const int ConfigVersion = 1;
    public const string SaveStart = "#SS#";
    public const string SaveEnd = "#SE#";
    public const string EntryEnd = "#EE#";

    public static LeaderboardsSave Instance { get; private set; } = CheckIfSaveExists() ? (Load() ?? new LeaderboardsSave()) : new LeaderboardsSave(saveLock: false);

    private static string GetDefaultPath() => "user://leaderboards.sav";
    private const string Password = "very_strong_password_yeah_this_is_fine_just_a_mock_xd";

    public static bool CheckIfSaveExists() => new File().FileExists(GetDefaultPath());

    public bool Save(string path = null)
    {
        if (SaveLock)
        {
            GD.PrintErr("LeaderboardsSave: SaveLock is enabled, can't save");
            return false;
        }

        if(path == null)
            path = GetDefaultPath();

        var file = new File();
        // file.OpenEncryptedWithPass(path, File.ModeFlags.Write, Password);
        file.Open(path, File.ModeFlags.Write);

        file.Store32(ConfigVersion);
        file.Store32((uint)Entries.Count);
        file.StoreLine(SaveStart);
        foreach(var entry in Entries)
        {
            file.Store64((ulong)entry.Date.Ticks);
            file.Store32((uint)entry.Score);
            file.Store32((uint)entry.DifficultyLevel);

            file.StoreLine(EntryEnd);
        }

        file.StoreLine(SaveEnd);
        file.Close();
        return true;
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

        var version = file.Get32();
        if(version != ConfigVersion)
        {
            GD.PrintErr($"Failed to load file {path}! Error: Invalid version");
            return null;
        }

        var count = (int)file.Get32();
        var entries = new List<Entry>(count);
        var start = file.GetLine();
        if (start != SaveStart)
        {
            GD.PrintErr($"Failed to load file {path}! Error: Invalid save start");
            return null;
        }

        for(var i = 0; i < count; i++)
        {
            var ticks = (long)file.Get64();
            var date = new DateTime(ticks);
            var score = (int)file.Get32();
            var difficulty = (int)file.Get32();
            var end = file.GetLine();
            if(end != EntryEnd)
            {
                GD.PrintErr($"Failed to load file {path}! Error: Invalid entry end");
                return null;
            }
            entries.Add(new Entry(date, score, difficulty));
        }

        var end2 = file.GetLine();
        if(end2 != SaveEnd)
        {
            GD.PrintErr($"Failed to load file {path}! Error: Invalid save end");
            return null;
        }
        file.Close();
        return new LeaderboardsSave(saveLock: false) { Entries = entries };
    }

    public LeaderboardsSave(bool saveLock = true)
    {
        this.SaveLock = saveLock;
    }
}
