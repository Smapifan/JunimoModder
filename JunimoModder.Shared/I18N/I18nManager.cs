using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace JunimoModder.Shared.I18N
{
    /// <summary>
    /// Represents one loaded language pack.
    /// </summary>
    public sealed class I18nLanguagePack
    {
        /// <summary>
        /// The language code, for example de or en.
        /// </summary>
        public string LanguageCode { get; init; } = string.Empty;

        /// <summary>
        /// The display name of the language.
        /// </summary>
        public string DisplayName { get; init; } = string.Empty;

        /// <summary>
        /// The language description.
        /// </summary>
        public string Description { get; init; } = string.Empty;

        /// <summary>
        /// All translation keys and their values.
        /// </summary>
        public Dictionary<string, string> Texts { get; } = new(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Loads and manages translations from embedded i18nCore files and a runtime i18n folder.
    /// </summary>
    public sealed class I18nManager
    {
        private readonly Dictionary<string, I18nLanguagePack> _packs = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// The currently active language code.
        /// </summary>
        public string CurrentLanguageCode { get; private set; } = NormalizeLanguageCode(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

        /// <summary>
        /// Loads translation files from embedded resources inside the application assembly.
        /// </summary>
        public void LoadEmbeddedCore()
        {
            LoadFromEmbeddedResources(typeof(I18nManager).Assembly, ".i18nCore.");
        }

        /// <summary>
        /// Loads all translation files from a folder.
        /// </summary>
        public void LoadFromFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                return;
            }

            foreach (var filePath in Directory.EnumerateFiles(folderPath, "*.json", SearchOption.AllDirectories))
            {
                LoadFromJsonFile(filePath);
            }
        }

        /// <summary>
        /// Loads translation files from embedded resources that match the given marker.
        /// </summary>
        public void LoadFromEmbeddedResources(Assembly assembly, string resourceMarker)
        {
            foreach (var resourceName in assembly.GetManifestResourceNames())
            {
                if (!resourceName.Contains(resourceMarker, StringComparison.OrdinalIgnoreCase) ||
                    !resourceName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream is null)
                {
                    continue;
                }

                using var reader = new StreamReader(stream);
                LoadFromJsonText(reader.ReadToEnd(), Path.GetFileNameWithoutExtension(resourceName));
            }
        }

        /// <summary>
        /// Loads a translation file from disk.
        /// </summary>
        public void LoadFromJsonFile(string filePath)
        {
            LoadFromJsonText(File.ReadAllText(filePath), Path.GetFileNameWithoutExtension(filePath));
        }

        /// <summary>
        /// Adds or replaces a language pack from JSON text.
        /// </summary>
        public void LoadFromJsonText(string jsonText, string fallbackLanguageCode)
        {
            using var document = JsonDocument.Parse(jsonText);
            var root = document.RootElement;

            var languageCode = NormalizeLanguageCode(GetString(root, "language", fallbackLanguageCode));
            var displayName = GetString(root, "displayName", languageCode);
            var description = GetString(root, "description", string.Empty);

            var pack = new I18nLanguagePack
            {
                LanguageCode = languageCode,
                DisplayName = displayName,
                Description = description
            };

            if (root.TryGetProperty("keys", out var keysElement) && keysElement.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in keysElement.EnumerateObject())
                {
                    pack.Texts[property.Name] = property.Value.GetString() ?? string.Empty;
                }
            }

            _packs[languageCode] = pack;

            if (_packs.Count == 1)
            {
                CurrentLanguageCode = languageCode;
            }
            else if (!_packs.ContainsKey(CurrentLanguageCode))
            {
                CurrentLanguageCode = languageCode;
            }
        }

        /// <summary>
        /// Sets the active language if the pack is available.
        /// </summary>
        public bool SetLanguage(string languageCode)
        {
            languageCode = NormalizeLanguageCode(languageCode);

            if (_packs.ContainsKey(languageCode))
            {
                CurrentLanguageCode = languageCode;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a translated text or the key itself as fallback.
        /// </summary>
        public string T(string key)
        {
            if (_packs.TryGetValue(CurrentLanguageCode, out var activePack) &&
                activePack.Texts.TryGetValue(key, out var translatedText))
            {
                return translatedText;
            }

            if (_packs.TryGetValue("en", out var englishPack) && englishPack.Texts.TryGetValue(key, out translatedText))
            {
                return translatedText;
            }

            foreach (var pack in _packs.Values)
            {
                if (pack.Texts.TryGetValue(key, out translatedText))
                {
                    return translatedText;
                }
            }

            return key;
        }

        /// <summary>
        /// Returns the currently loaded languages.
        /// </summary>
        public IReadOnlyCollection<string> GetAvailableLanguages() => _packs.Keys.ToArray();

        private static string GetString(JsonElement root, string propertyName, string fallbackValue)
        {
            return root.TryGetProperty(propertyName, out var element) ? element.GetString() ?? fallbackValue : fallbackValue;
        }

        private static string NormalizeLanguageCode(string? languageCode)
        {
            languageCode = (languageCode ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(languageCode))
            {
                return "en";
            }

            var dashIndex = languageCode.IndexOf('-');
            return dashIndex > 0 ? languageCode[..dashIndex].ToLowerInvariant() : languageCode.ToLowerInvariant();
        }
    }
}
