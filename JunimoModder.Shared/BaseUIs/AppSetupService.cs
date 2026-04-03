using System;
using System.IO;
using JunimoModder.Shared.I18N;

namespace JunimoModder.Shared.BaseUIs
{
    /// <summary>
    /// Creates the application root structure on first start.
    /// </summary>
    public static class AppSetupService
    {
        private const string AppFolderName = "JunimoModder";

        /// <summary>
        /// Gets the application root path below the selected parent path.
        /// </summary>
        public static string GetRootDirectory(string selectedParentDirectory)
        {
            return Path.Combine(selectedParentDirectory, AppFolderName);
        }

        /// <summary>
        /// Creates the root folder structure and copies the embedded i18nCore files to the runtime i18n folder.
        /// </summary>
        public static bool ConfigureRootDirectory(string selectedParentDirectory, out string rootDirectory, out string errorMessage)
        {
            rootDirectory = GetRootDirectory(selectedParentDirectory);
            errorMessage = string.Empty;

            try
            {
                Directory.CreateDirectory(rootDirectory);
                Directory.CreateDirectory(AppEnvironment.GetI18nDirectory(rootDirectory));
                Directory.CreateDirectory(AppEnvironment.GetFrameworksDirectory(rootDirectory));
                Directory.CreateDirectory(AppEnvironment.GetProjectsDirectory(rootDirectory));
                Directory.CreateDirectory(AppEnvironment.GetContentDirectory(rootDirectory));

                CopyEmbeddedI18nCoreFiles(rootDirectory);

                AppEnvironment.SetRootDirectory(rootDirectory);
                AppEnvironment.I18n.LoadFromFolder(AppEnvironment.GetI18nDirectory(rootDirectory));
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Copies all embedded i18nCore JSON files into the runtime i18n folder.
        /// </summary>
        public static void CopyEmbeddedI18nCoreFiles(string rootDirectory)
        {
            var targetFolder = AppEnvironment.GetI18nDirectory(rootDirectory);
            Directory.CreateDirectory(targetFolder);

            var assembly = typeof(AppSetupService).Assembly;
            foreach (var resourceName in assembly.GetManifestResourceNames())
            {
                if (!resourceName.Contains(".i18nCore.", StringComparison.OrdinalIgnoreCase) ||
                    !resourceName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var fileName = ExtractFileName(resourceName);
                var targetPath = Path.Combine(targetFolder, fileName);

                using var source = assembly.GetManifestResourceStream(resourceName);
                if (source is null)
                {
                    continue;
                }

                using var destination = File.Create(targetPath);
                source.CopyTo(destination);
            }
        }

        private static string ExtractFileName(string resourceName)
        {
            var segments = resourceName.Split('.');
            if (segments.Length < 2)
            {
                return resourceName;
            }

            return $"{segments[^2]}.{segments[^1]}";
        }
    }
}
