using System;
using System.IO;
using JunimoModder.Shared.I18N;

namespace JunimoModder.Shared.BaseUIs
{
    /// <summary>
    /// Provides application-wide runtime state and helpers.
    /// </summary>
    public static class AppEnvironment
    {
        /// <summary>
        /// Current persistent settings.
        /// </summary>
        public static AppSettings Settings { get; private set; } = new();

        /// <summary>
        /// Shared translation engine.
        /// </summary>
        public static I18nManager I18n { get; } = new();

        /// <summary>
        /// Initializes the runtime state and loads translations.
        /// </summary>
        public static void Initialize()
        {
            Settings = AppSettingsStore.Load();
            I18n.LoadEmbeddedCore();

            if (IsConfigured)
            {
                var runtimeI18nFolder = GetI18nDirectory(Settings.RootDirectory);
                I18n.LoadFromFolder(runtimeI18nFolder);
            }

            // Wenn "auto" oder nicht gesetzt: Systemsprache nutzen
            if (string.Equals(Settings.PreferredLanguageCode, "auto", StringComparison.OrdinalIgnoreCase) ||
                string.IsNullOrWhiteSpace(Settings.PreferredLanguageCode))
            {
                // Systemsprache nutzen (bereits im I18nManager via CultureInfo.CurrentUICulture)
                Settings.PreferredLanguageCode = "auto";
            }
            else
            {
                I18n.SetLanguage(Settings.PreferredLanguageCode);
            }
        }

        /// <summary>
        /// Returns true if the application root has already been configured.
        /// </summary>
        public static bool IsConfigured => !string.IsNullOrWhiteSpace(Settings.RootDirectory) && Directory.Exists(Settings.RootDirectory);

        /// <summary>
        /// Gets the configured application root directory.
        /// </summary>
        public static string RootDirectory => Settings.RootDirectory;

        /// <summary>
        /// Gets the i18n directory for the configured root.
        /// </summary>
        public static string GetI18nDirectory(string rootDirectory) => Path.Combine(rootDirectory, "i18n");

        /// <summary>
        /// Gets the frameworks directory for the configured root.
        /// </summary>
        public static string GetFrameworksDirectory(string rootDirectory) => Path.Combine(rootDirectory, "Frameworks");

        /// <summary>
        /// Gets the projects directory for the configured root.
        /// </summary>
        public static string GetProjectsDirectory(string rootDirectory) => Path.Combine(rootDirectory, "Projects");

        /// <summary>
        /// Gets the content directory for the configured root.
        /// </summary>
        public static string GetContentDirectory(string rootDirectory) => Path.Combine(rootDirectory, "Content");

        /// <summary>
        /// Returns a translated text for the current language.
        /// </summary>
        public static string T(string key) => I18n.T(key);

        /// <summary>
        /// Updates and persists the configured root directory.
        /// </summary>
        public static void SetRootDirectory(string rootDirectory)
        {
            Settings.RootDirectory = rootDirectory;
            AppSettingsStore.Save(Settings);
        }

        /// <summary>
        /// Updates and persists the preferred language.
        /// </summary>
        public static void SetPreferredLanguage(string languageCode)
        {
            Settings.PreferredLanguageCode = languageCode;
            AppSettingsStore.Save(Settings);
            I18n.SetLanguage(languageCode);
        }
    }
}
