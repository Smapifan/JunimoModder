using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace JunimoModder.Shared.FrameworkLoader
{
    /// <summary>
    /// Loads and analyzes frameworks from the Frameworks/ folder.
    /// Detects framework roots by manifest.json and loads related UI/ and Templates/ (JSON-based).
    /// Supports unlimited framework roots and I18N integration.
    /// </summary>
    public class FrameworkManager
    {
        /// <summary>
        /// List of all loaded framework roots.
        /// </summary>
        public List<FrameworkRoot> Frameworks { get; } = new();

        /// <summary>
        /// Searches and loads all framework roots in the Frameworks/ directory.
        /// </summary>
        /// <param name="frameworksPath">Path to the Frameworks/ directory.</param>
        public void LoadAll(string frameworksPath)
        {
            // TODO: Recursively search for manifest.json, create FrameworkRoot objects
        }
    }

    /// <summary>
    /// Represents a framework root with UI and templates.
    /// </summary>
    public class FrameworkRoot
    {
        /// <summary>
        /// The root path of the framework.
        /// </summary>
        public string RootPath { get; set; } = string.Empty;

        /// <summary>
        /// The manifest.json document for this framework.
        /// </summary>
        public JsonDocument? Manifest { get; set; }

        /// <summary>
        /// List of UI JSON files in this framework.
        /// </summary>
        public List<string> UiFiles { get; set; } = new();

        /// <summary>
        /// List of template JSON files in this framework.
        /// </summary>
        public List<string> TemplateFiles { get; set; } = new();

        /// <summary>
        /// List of I18N JSON files in this framework.
        /// </summary>
        public List<string> I18nFiles { get; set; } = new();
    }
}
