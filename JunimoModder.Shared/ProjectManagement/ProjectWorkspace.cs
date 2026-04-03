using System;
using System.Collections.Generic;
using JunimoModder.Shared.BaseUIs;

namespace JunimoModder.Shared.ProjectManagement
{
    /// <summary>
    /// Represents the current project state used by the UI.
    /// </summary>
    public sealed class ProjectWorkspace
    {
        /// <summary>
        /// The currently loaded or edited project manifest.
        /// </summary>
        public ProjectManifest CurrentProject { get; private set; } = ProjectManager.CreateManifest("NewMod");

        /// <summary>
        /// The current project folder path.
        /// </summary>
        public string CurrentProjectFolder { get; private set; } = string.Empty;

        /// <summary>
        /// A short status message for the UI.
        /// </summary>
        public string StatusMessage { get; private set; } = AppEnvironment.T("project.status_ready");

        /// <summary>
        /// Returns the list of available project summaries.
        /// </summary>
        public IReadOnlyList<ProjectSummary> GetProjects() => new List<ProjectSummary>(ProjectManager.GetProjects());

        /// <summary>
        /// Creates a new empty project in memory.
        /// </summary>
        public void NewProject(string name, string description)
        {
            CurrentProject = ProjectManager.CreateManifest(name, description);
            CurrentProjectFolder = string.Empty;
            StatusMessage = AppEnvironment.T("project.status_new_project");
        }

        /// <summary>
        /// Loads a project from a folder that contains a manifest.json file.
        /// </summary>
        public bool LoadProject(string projectFolder)
        {
            var manifestPath = ProjectManager.GetManifestPath(projectFolder);
            var loadedProject = ProjectManager.LoadManifest(manifestPath);

            if (loadedProject is null)
            {
                StatusMessage = AppEnvironment.T("project.status_invalid");
                return false;
            }

            CurrentProject = loadedProject;
            CurrentProjectFolder = projectFolder;
            StatusMessage = AppEnvironment.T("project.status_loaded");
            return true;
        }

        /// <summary>
        /// Saves the current project to disk.
        /// </summary>
        public bool SaveProject(bool replaceExisting, out string? duplicateFolder)
        {
            try
            {
                var success = ProjectManager.SaveOrImportProject(CurrentProject, CurrentProjectFolder, replaceExisting, out var savedFolder, out duplicateFolder);

                if (!success)
                {
                    StatusMessage = AppEnvironment.T("project.status_duplicate_found");
                    return false;
                }

                CurrentProjectFolder = savedFolder;
                StatusMessage = AppEnvironment.T("project.status_saved");
                return true;
            }
            catch (Exception ex)
            {
                duplicateFolder = null;
                StatusMessage = $"{AppEnvironment.T("project.status_save_failed")}: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Imports a project from a manifest file.
        /// </summary>
        public bool ImportProject(string manifestPath, bool replaceExisting, out string? duplicateFolder)
        {
            duplicateFolder = null;

            try
            {
                var importedProject = ProjectManager.LoadManifest(manifestPath);
                if (importedProject is null)
                {
                    StatusMessage = AppEnvironment.T("project.status_invalid");
                    return false;
                }

                CurrentProject = importedProject;
                var success = ProjectManager.SaveOrImportProject(CurrentProject, null, replaceExisting, out var savedFolder, out duplicateFolder);

                if (!success)
                {
                    StatusMessage = AppEnvironment.T("project.status_duplicate_found");
                    return false;
                }

                CurrentProjectFolder = savedFolder;
                StatusMessage = AppEnvironment.T("project.status_imported");
                return true;
            }
            catch (Exception ex)
            {
                StatusMessage = $"{AppEnvironment.T("project.status_import_failed")}: {ex.Message}";
                return false;
            }
        }
    }
}
