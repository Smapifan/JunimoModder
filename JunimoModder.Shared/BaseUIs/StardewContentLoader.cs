using System;
using System.IO;

namespace JunimoModder.Shared.BaseUIs
{
    /// <summary>
    /// Provides access to Stardew standard data from the configured Content folder.
    /// </summary>
    public static class StardewContentLoader
    {
        /// <summary>
        /// Loads a file from the runtime Content directory as a string.
        /// </summary>
        public static string? LoadContentFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(AppEnvironment.RootDirectory))
            {
                return null;
            }

            var path = Path.Combine(AppEnvironment.GetContentDirectory(AppEnvironment.RootDirectory), fileName);
            if (!File.Exists(path))
            {
                return null;
            }

            return File.ReadAllText(path);
        }
    }
}
