using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using JunimoModder.Shared.BaseUIs;

namespace JunimoModder.Shared.ProjectManagement
{
    /// <summary>
    /// Handles project creation, loading, saving, and import based on manifest.json.
    /// </summary>
    public static class ProjectManager
    {
        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        /// <summary>
        /// Returns the absolute Projects folder path.
        /// </summary>
        public static string ProjectsRoot => AppEnvironment.IsConfigured
            ? AppEnvironment.GetProjectsDirectory(AppEnvironment.RootDirectory)
            : string.Empty;

        /// <summary>
        /// Creates a new manifest with default metadata.
        /// </summary>
        public static ProjectManifest CreateManifest(string name, string description = "")
        {
            var now = DateTime.UtcNow;
            var resolvedName = string.IsNullOrWhiteSpace(name) ? "NewMod" : name.Trim();
            var resolvedAuthor = SanitizeIdentifierPart(Environment.UserName);
            var resolvedUniqueId = $"{resolvedAuthor}.{SanitizeIdentifierPart(resolvedName)}.{Guid.NewGuid().ToString("N")[..6]}";

            return new ProjectManifest
            {
                Name = resolvedName,
                Author = string.IsNullOrWhiteSpace(Environment.UserName) ? "Unknown" : Environment.UserName,
                UniqueID = resolvedUniqueId,
                Description = description.Trim(),
                CreatedUtc = now,
                UpdatedUtc = now
            };
        }

        /// <summary>
        /// Returns the manifest path for a project folder.
        /// </summary>
        public static string GetManifestPath(string projectFolder) => Path.Combine(projectFolder, "manifest.json");

        /// <summary>
        /// Returns a safe folder name for the given manifest.
        /// </summary>
        public static string GetProjectFolderName(ProjectManifest manifest)
        {
            var namePart = SanitizeFileName(string.IsNullOrWhiteSpace(manifest.Name) ? "Project" : manifest.Name);
            var idPart = manifest.ProjectId.Length >= 8 ? manifest.ProjectId[..8] : manifest.ProjectId;
            return $"{namePart}_{idPart}";
        }

        /// <summary>
        /// Loads a manifest from disk.
        /// </summary>
        public static ProjectManifest? LoadManifest(string manifestPath)
        {
            if (!File.Exists(manifestPath))
            {
                return null;
            }

            return JsonSerializer.Deserialize<ProjectManifest>(File.ReadAllText(manifestPath), JsonOptions);
        }

        /// <summary>
        /// Saves a manifest to the given project folder.
        /// </summary>
        public static void SaveManifest(ProjectManifest manifest, string projectFolder)
        {
            Directory.CreateDirectory(projectFolder);
            manifest.UpdatedUtc = DateTime.UtcNow;
            File.WriteAllText(GetManifestPath(projectFolder), JsonSerializer.Serialize(manifest, JsonOptions));
            EnsureContentJson(projectFolder);
        }

        /// <summary>
        /// Loads all project manifests found below the Projects folder.
        /// </summary>
        public static IEnumerable<ProjectSummary> GetProjects()
        {
            if (string.IsNullOrWhiteSpace(ProjectsRoot) || !Directory.Exists(ProjectsRoot))
            {
                yield break;
            }

            foreach (var manifestPath in Directory.EnumerateFiles(ProjectsRoot, "manifest.json", SearchOption.AllDirectories))
            {
                var manifest = LoadManifest(manifestPath);
                if (manifest is null)
                {
                    continue;
                }

                yield return new ProjectSummary
                {
                    FolderPath = Path.GetDirectoryName(manifestPath) ?? string.Empty,
                    ManifestPath = manifestPath,
                    Manifest = manifest
                };
            }
        }

        /// <summary>
        /// Finds a project folder that already contains an identical manifest file.
        /// </summary>
        public static string? FindIdenticalProjectFolder(ProjectManifest manifest)
        {
            if (string.IsNullOrWhiteSpace(ProjectsRoot) || !Directory.Exists(ProjectsRoot))
            {
                return null;
            }

            var manifestJson = JsonSerializer.Serialize(manifest, JsonOptions);

            foreach (var manifestPath in Directory.EnumerateFiles(ProjectsRoot, "manifest.json", SearchOption.AllDirectories))
            {
                var existingManifest = LoadManifest(manifestPath);
                if (existingManifest is null)
                {
                    continue;
                }

                var existingManifestJson = JsonSerializer.Serialize(existingManifest, JsonOptions);
                if (string.Equals(existingManifestJson, manifestJson, StringComparison.Ordinal))
                {
                    return Path.GetDirectoryName(manifestPath);
                }
            }

            return null;
        }

        /// <summary>
        /// Saves or imports a project manifest.
        /// </summary>
        public static bool SaveOrImportProject(ProjectManifest manifest, string? targetFolder, bool replaceExisting, out string savedFolder, out string? identicalFolder)
        {
            identicalFolder = FindIdenticalProjectFolder(manifest);

            if (identicalFolder is not null && !replaceExisting)
            {
                savedFolder = identicalFolder;
                return false;
            }

            savedFolder = !string.IsNullOrWhiteSpace(identicalFolder) && replaceExisting
                ? identicalFolder
                : string.IsNullOrWhiteSpace(targetFolder)
                    ? Path.Combine(ProjectsRoot, GetProjectFolderName(manifest))
                    : targetFolder;

            SaveManifest(manifest, savedFolder);
            return true;
        }

        private static string SanitizeFileName(string value)
        {
            foreach (var invalidChar in Path.GetInvalidFileNameChars())
            {
                value = value.Replace(invalidChar, '_');
            }

            return value.Trim();
        }

        private static string SanitizeIdentifierPart(string value)
        {
            var cleaned = string.IsNullOrWhiteSpace(value) ? "User" : value.Trim();
            var chars = cleaned.ToCharArray();

            for (var i = 0; i < chars.Length; i++)
            {
                var c = chars[i];
                if (!(char.IsLetterOrDigit(c) || c == '_' || c == '.'))
                {
                    chars[i] = '_';
                }
            }

            return new string(chars);
        }

        private static void EnsureContentJson(string projectFolder)
        {
            var contentJsonPath = Path.Combine(projectFolder, "content.json");
            if (File.Exists(contentJsonPath))
            {
                return;
            }

            const string defaultContentJson = "{\n  \"Format\": \"2.0.0\",\n  \"Changes\": []\n}";
            File.WriteAllText(contentJsonPath, defaultContentJson);
        }
    }
}
