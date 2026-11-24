using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;

namespace PetPulse
{
    public class AppSettings
    {
        public bool LightTheme { get; set; } = false;
        public bool NotifyToday { get; set; } = true;
        public bool ConfirmExit { get; set; } = true;
        public int Language { get; set; } = 0;
        public bool DeveloperMode { get; set; } = false;
    }

    public static class AppSettingsStore
    {
        private static readonly string FilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PetPulse", "settings.json");

        public static AppSettings Current { get; private set; } = new AppSettings();

        public static void Load()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var s = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(FilePath));
                    if (s != null) Current = s;
                }
            }
            catch { }
        }

        public static void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
                File.WriteAllText(FilePath, JsonSerializer.Serialize(Current, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch { }
        }
    }

    public static class ThemeManager
    {
        public static event Action? ThemeChanged;
        public static bool IsLight { get; private set; }
        private static bool _loaded;

        private static readonly Dictionary<string, string> Dark = new Dictionary<string, string>
        {
            ["BgDarkBrush"] = "#14181B",
            ["CardBgBrush"] = "#1D2326",
            ["InnerBgBrush"] = "#262E32",
            ["SettingsBgBrush"] = "#181D20",
            ["LineBrush"] = "#2E383D",
            ["LineStrongBrush"] = "#3B484E",
            ["AccentMintBrush"] = "#3DDBA8",
            ["AccentEmeraldBrush"] = "#1B8A6B",
            ["DangerBrush"] = "#A8294A",
            ["TextPrimaryBrush"] = "#EAF2F0",
            ["TextSecondaryBrush"] = "#A9B8B5",
            ["TextMutedBrush"] = "#7C8C89",
            ["TextDisabledBrush"] = "#5A6664",
            ["InputBgBrush"] = "#F4F7F6",
            ["InputFgBrush"] = "#111111",
            ["InputBorderBrush"] = "#C9D3D0",
            ["ChipBgBrush"] = "#24403A",
            ["ToggleOffBrush"] = "#55605E",
            ["PawBrush"] = "#2A3438",
            ["WarnBrush"] = "#E8B04A",
            ["AlertBrush"] = "#FF6B6B",
        };

        private static readonly Dictionary<string, string> Light = new Dictionary<string, string>
        {
            ["BgDarkBrush"] = "#F3F7F6",
            ["CardBgBrush"] = "#FFFFFF",
            ["InnerBgBrush"] = "#EDF3F1",
            ["SettingsBgBrush"] = "#EAF1EF",
            ["LineBrush"] = "#D9E3E0",
            ["LineStrongBrush"] = "#BFCFCB",
            ["AccentMintBrush"] = "#12806A",
            ["AccentEmeraldBrush"] = "#1B8A6B",
            ["DangerBrush"] = "#C0392B",
            ["TextPrimaryBrush"] = "#1B2523",
            ["TextSecondaryBrush"] = "#44524F",
            ["TextMutedBrush"] = "#6B7B77",
            ["TextDisabledBrush"] = "#A3B0AD",
            ["InputBgBrush"] = "#FFFFFF",
            ["InputFgBrush"] = "#111111",
            ["InputBorderBrush"] = "#B8C6C2",
            ["ChipBgBrush"] = "#DDEEE9",
            ["ToggleOffBrush"] = "#9AA8A5",
            ["PawBrush"] = "#D3E0DC",
            ["WarnBrush"] = "#B7791F",
            ["AlertBrush"] = "#C0392B",
        };

        public static void Init()
        {
            if (!_loaded)
            {
                AppSettingsStore.Load();
                _loaded = true;
            }
            Build(AppSettingsStore.Current.LightTheme);
        }

        public static void Apply(bool light)
        {
            AppSettingsStore.Current.LightTheme = light;
            AppSettingsStore.Save();
            Build(light);
            ThemeChanged?.Invoke();
        }

        private static void Build(bool light)
        {
            IsLight = light;
            var palette = light ? Light : Dark;
            foreach (var kv in palette)
            {
                var color = (Color)ColorConverter.ConvertFromString(kv.Value);
                Application.Current.Resources[kv.Key] = new SolidColorBrush(color);
            }
        }

        public static Brush GetBrush(string key)
        {
            return Application.Current.Resources[key] as Brush ?? Brushes.Gray;
        }
    }
}