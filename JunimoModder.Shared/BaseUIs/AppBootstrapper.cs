namespace JunimoModder.Shared.BaseUIs
{
    /// <summary>
    /// Performs startup initialization for the application.
    /// </summary>
    public static class AppBootstrapper
    {
        /// <summary>
        /// Initializes the shared runtime state.
        /// </summary>
        public static void Initialize()
        {
            AppEnvironment.Initialize();
        }

        /// <summary>
        /// Returns true when the first-run setup has not been completed yet.
        /// </summary>
        public static bool NeedsFirstRunSetup => !AppEnvironment.IsConfigured;
    }
}
