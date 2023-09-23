using Godot;
using System;

public class Config
{
    public int DifficultyLevel { get; set; }

    private static string GetDefaultPath() => "user://config.cfg";

    public static Config Instance { get; private set; } = Load();

    public void Save(string path = null)
    {
        if(path == null)
            path = GetDefaultPath();

        var config = new ConfigFile();
        config.SetValue("Config", "Level", DifficultyLevel);

        config.Save(path);
    }

    public static Config Load(string path = null)
    {
        if(path == null)
            path = GetDefaultPath();

        var config = new ConfigFile();
        config.Load(path);

        var cfg = new Config();
        cfg.DifficultyLevel = (int)config.GetValue("Config", "Level", 1);

        return cfg;
    }
}
