using System;

namespace JunimoModder.Shared.ProjectManagement
{
    /// <summary>
    /// Stores the metadata of one JunimoModder project.
    /// </summary>
    public sealed class ProjectManifest
    {
        /// <summary>
        /// SMAPI project name shown in game and tools.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// SMAPI author.
        /// </summary>
        public string Author { get; set; } = string.Empty;

        /// <summary>
        /// SMAPI semantic version.
        /// </summary>
        public string Version { get; set; } = "1.0.0";

        /// <summary>
        /// SMAPI unique identifier.
        /// </summary>
        public string UniqueID { get; set; } = string.Empty;

        /// <summary>
        /// SMAPI description.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Optional update keys for release channels.
        /// </summary>
        public string[] UpdateKeys { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Optional dependency declaration for Content Patcher packs.
        /// </summary>
        public SmapiContentPackFor? ContentPackFor { get; set; } = new SmapiContentPackFor
        {
            UniqueID = "Pathoschild.ContentPatcher",
            MinimumVersion = "2.0.0"
        };

        /// <summary>
        /// Internal unique project identifier.
        /// </summary>
        public string ProjectId { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// The UTC timestamp when the project was created.
        /// </summary>
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The UTC timestamp when the project was last modified.
        /// </summary>
        public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Internal manifest format version.
        /// </summary>
        public int InternalManifestVersion { get; set; } = 1;
    }

    /// <summary>
    /// Represents the SMAPI ContentPackFor object.
    /// </summary>
    public sealed class SmapiContentPackFor
    {
        /// <summary>
        /// Target mod unique id.
        /// </summary>
        public string UniqueID { get; set; } = string.Empty;

        /// <summary>
        /// Minimum supported target version.
        /// </summary>
        public string MinimumVersion { get; set; } = string.Empty;
    }
}
