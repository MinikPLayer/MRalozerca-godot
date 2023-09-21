using System.Collections.Generic;
using Godot;

namespace MRalozerca2.Scripts
{
    // TODO: Add different difficulty levels with more colors
    public enum GameColors
    {
        Red = 0,
        Blue = 1,
        Green = 2,
    }

    public static class ColorManager
    {
        public static int EnabledColorsCount = 2;

        static readonly Dictionary<GameColors, Color> Colors = new Dictionary<GameColors, Color>()
        {
            {GameColors.Red, new Color(1, 0, 0)},
            {GameColors.Green, new Color(0, 1, 0)},
            {GameColors.Blue, new Color(0, 0, 1)},
        };

        public static Color ToColor(this GameColors c)
        {
            if (!Colors.ContainsKey(c))
                throw new KeyNotFoundException($"Color {c} not found!");

            return Colors[c];
        }

        public static GameColors ToGameColor(this Color c)
        {
            foreach (var color in Colors)
            {
                if (color.Value == c)
                    return color.Key;
            }

            throw new KeyNotFoundException($"Color {c} not found!");
        }

        public static GameColors NextColor(this GameColors c) => (GameColors) (((int) c + 1) % EnabledColorsCount);
        public static GameColors PreviousColor(this GameColors c) => (GameColors) (((int) c + 2) % EnabledColorsCount);

        public static GameColors RandomGameColor() => (GameColors)(GD.Randi() % EnabledColorsCount);
    }
}