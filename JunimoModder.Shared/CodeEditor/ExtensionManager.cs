using System;
using System.Collections.Generic;

namespace JunimoModder.Shared.CodeEditor
{
    /// <summary>
    /// Manages and loads editor extensions.
    /// </summary>
    public class ExtensionManager
    {
        // ===================== Fields =====================

        /// <summary>
        /// List of installed extension paths.
        /// </summary>
        private readonly List<string> _installedExtensions = new();

        // ===================== Methods =====================

        /// <summary>
        /// Installs a new extension.
        /// </summary>
        /// <param name="extensionPath">Path to the extension file or folder.</param>
        public void Install(string extensionPath)
        {
            // TODO: Load and register the extension
            _installedExtensions.Add(extensionPath);
        }

        /// <summary>
        /// Returns all installed extensions.
        /// </summary>
        public IEnumerable<string> GetInstalledExtensions() => _installedExtensions;
    }
}
