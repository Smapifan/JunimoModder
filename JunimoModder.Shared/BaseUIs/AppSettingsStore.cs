using System;
using System.IO;
using System.Text.Json;

namespace JunimoModder.Shared.BaseUIs
{
    /// <summary>
    /// Loads and saves persistent application settings.
    /// </summary>
    public static class AppSettingsStore
    {
        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        /// <summary>
        /// Returns the file path used for the settings file.
        /// </summary>
        public static string SettingsPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "JunimoModder",
            "appsettings.json");

        /// <summary>
        /// Loads settings from disk or returns defaults if nothing exists.
        /// </summary>
        public static AppSettings Load()
        {
            if (!File.Exists(SettingsPath))
            {
                return new AppSettings();
            }

            var json = File.ReadAllText(SettingsPath);
            return JsonSerializer.Deserialize<AppSettings>(json, JsonOptions) ?? new AppSettings();
        }

        /// <summary>
        /// Saves settings to disk.
        /// </summary>
        public static void Save(AppSettings settings)
        {
            var settingsDirectory = Path.GetDirectoryName(SettingsPath);
            if (!string.IsNullOrWhiteSpace(settingsDirectory))
            {
                Directory.CreateDirectory(settingsDirectory);
            }

            File.WriteAllText(SettingsPath, JsonSerializer.Serialize(settings, JsonOptions));
        }
    }
}
