namespace JunimoModder.Shared.ProjectManagement
{
    /// <summary>
    /// Lightweight project information used for lists.
    /// </summary>
    public sealed class ProjectSummary
    {
        /// <summary>
        /// The path to the project folder.
        /// </summary>
        public string FolderPath { get; set; } = string.Empty;

        /// <summary>
        /// The path to the manifest file.
        /// </summary>
        public string ManifestPath { get; set; } = string.Empty;

        /// <summary>
        /// The loaded manifest data.
        /// </summary>
        public ProjectManifest Manifest { get; set; } = new();

        /// <summary>
        /// Convenience display text for the UI.
        /// </summary>
        public string DisplayText => string.IsNullOrWhiteSpace(Manifest.Name)
            ? FolderPath
            : Manifest.Name;

        /// <summary>
        /// Convenience subtitle for project lists.
        /// </summary>
        public string SubtitleText => string.IsNullOrWhiteSpace(Manifest.Description)
            ? FolderPath
            : Manifest.Description;

        /// <summary>
        /// Human-readable last update timestamp.
        /// </summary>
        public string UpdatedText => Manifest.UpdatedUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
    }
}
