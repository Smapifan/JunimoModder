namespace JunimoModder.Shared.BaseUIs
{
    /// <summary>
    /// Stores persistent application settings.
    /// </summary>
    public sealed class AppSettings
    {
        /// <summary>
        /// The selected application root directory.
        /// </summary>
        public string RootDirectory { get; set; } = string.Empty;

        /// <summary>
        /// The preferred language code or auto.
        /// </summary>
        public string PreferredLanguageCode { get; set; } = "auto";
    }
}
