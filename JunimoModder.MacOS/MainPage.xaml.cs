using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using JunimoModder.Shared.BaseUIs;
using JunimoModder.Shared.ProjectManagement;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace JunimoModder.MacOS;

public partial class MainPage : ContentPage
{
    private readonly ProjectWorkspace _workspace = new();
    private readonly ObservableCollection<ProjectSummary> _projects = new();
    private readonly Entry _setupPathEntry = new();
    private readonly Label _setupStatusLabel = new();
    private readonly Entry _projectNameEntry = new();
    private readonly Entry _projectDescriptionEntry = new();
    private readonly Label _projectFolderLabel = new();
    private readonly Label _projectStatusLabel = new();
    private readonly CollectionView _projectsView = new();
    private readonly Picker _languagePicker = new();
    private StackLayout? _setupOverlay;

    public MainPage()
    {
        Title = AppEnvironment.T("app.title");
        BuildScreen();
    }

    private void BuildScreen()
    {
        ReloadProjects();
        var projectView = BuildProjectView();
        
        if (AppBootstrapper.NeedsFirstRunSetup)
        {
            Content = BuildSetupOverlay(projectView);
            return;
        }

        Content = projectView;
        _setupOverlay = null;
    }

    private View BuildSetupOverlay(View backgroundView)
    {
        _setupPathEntry.Text = GetSuggestedParentPath();
        _setupPathEntry.Placeholder = AppEnvironment.T("setup.root_placeholder");
        _setupStatusLabel.Text = AppEnvironment.T("setup.status_ready");

        ConfigureLanguagePicker();

        var browseButton = new Button { Text = AppEnvironment.T("setup.browse_folder") };
        browseButton.Clicked += (_, _) => OpenFolderBrowser();

        var configureButton = new Button { Text = AppEnvironment.T("setup.configure_root") };
        configureButton.Clicked += async (_, _) => await ConfigureRootAsync();

        _setupOverlay = new StackLayout
        {
            Padding = 24,
            Spacing = 16,
            Children =
            {
                new Label { Text = AppEnvironment.T("setup.title"), FontSize = 30, FontAttributes = FontAttributes.Bold },
                new Label { Text = AppEnvironment.T("setup.description"), Opacity = 0.85 },
                new Border
                {
                    StrokeThickness = 1,
                    Padding = 16,
                    Content = new VerticalStackLayout
                    {
                        Spacing = 12,
                        Children =
                        {
                            new Label { Text = AppEnvironment.T("project.language_label"), FontAttributes = FontAttributes.Bold },
                            _languagePicker,
                            new Label { Text = AppEnvironment.T("setup.root_label"), FontAttributes = FontAttributes.Bold },
                            _setupPathEntry,
                            new HorizontalStackLayout
                            {
                                Spacing = 12,
                                Children = { browseButton, configureButton }
                            }
                        }
                    }
                },
                new Label { Text = AppEnvironment.T("project.status_label"), FontAttributes = FontAttributes.Bold },
                _setupStatusLabel
            }
        };

        var scrollView = new ScrollView { Content = _setupOverlay };

        // Modal Overlay: Halbdunkles Hintergrund-Overlay
        var overlay = new Grid
        {
            RowDefinitions = { new RowDefinition { Height = GridLength.Star } },
            ColumnDefinitions = { new ColumnDefinition { Width = GridLength.Star } },
            BackgroundColor = Color.FromRgba(0, 0, 0, 0.6),
            Children = { scrollView }
        };

        return new Grid
        {
            RowDefinitions = { new RowDefinition { Height = GridLength.Star } },
            ColumnDefinitions = { new ColumnDefinition { Width = GridLength.Star } },
            Children = { backgroundView, overlay }
        };
    }

    private View BuildProjectView()
    {
        ConfigureLanguagePicker();
        _projectNameEntry.Text = _workspace.CurrentProject.Name;
        _projectNameEntry.Placeholder = AppEnvironment.T("project.name_placeholder");
        _projectDescriptionEntry.Text = _workspace.CurrentProject.Description;
        _projectDescriptionEntry.Placeholder = AppEnvironment.T("project.description_placeholder");
        _projectFolderLabel.Text = string.IsNullOrWhiteSpace(_workspace.CurrentProjectFolder)
            ? ProjectManager.ProjectsRoot
            : _workspace.CurrentProjectFolder;
        _projectStatusLabel.Text = _workspace.StatusMessage;

        _projectsView.ItemsSource = _projects;
        _projectsView.SelectionMode = SelectionMode.Single;
        _projectsView.SelectionChanged += (_, args) =>
        {
            if (args.CurrentSelection.FirstOrDefault() is ProjectSummary selectedProject)
            {
                LoadProject(selectedProject);
            }
        };
        _projectsView.ItemTemplate = new DataTemplate(() =>
        {
            var titleLabel = new Label { FontAttributes = FontAttributes.Bold };
            titleLabel.SetBinding(Label.TextProperty, nameof(ProjectSummary.DisplayText));
            var subtitleLabel = new Label { Opacity = 0.85 };
            subtitleLabel.SetBinding(Label.TextProperty, nameof(ProjectSummary.SubtitleText));
            var updatedLabel = new Label { FontSize = 12, Opacity = 0.7 };
            updatedLabel.SetBinding(Label.TextProperty, nameof(ProjectSummary.UpdatedText));

            return new Border
            {
                StrokeThickness = 1,
                Padding = 12,
                Margin = new Thickness(0, 4),
                Content = new VerticalStackLayout
                {
                    Spacing = 4,
                    Children = { titleLabel, subtitleLabel, updatedLabel }
                }
            };
        });

        var newProjectButton = new Button { Text = AppEnvironment.T("project.new_project") };
        newProjectButton.Clicked += (_, _) => CreateProject();

        var saveProjectButton = new Button { Text = AppEnvironment.T("project.save_project") };
        saveProjectButton.Clicked += async (_, _) => await SaveProjectAsync();

        var importButton = new Button { Text = AppEnvironment.T("project.import_manifest") };
        importButton.Clicked += async (_, _) => await ImportProjectAsync();

        var reloadButton = new Button { Text = AppEnvironment.T("project.reload_projects") };
        reloadButton.Clicked += (_, _) => ReloadProjects();

        return new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 24,
                Spacing = 16,
                Children =
                {
                    new Label { Text = AppEnvironment.T("project.title"), FontSize = 30, FontAttributes = FontAttributes.Bold },
                    new Label { Text = AppEnvironment.T("project.subtitle"), Opacity = 0.85 },
                    new Border
                    {
                        StrokeThickness = 1,
                        Padding = 16,
                        Content = new VerticalStackLayout
                        {
                            Spacing = 10,
                            Children =
                            {
                                new Label { Text = AppEnvironment.T("project.section_setup"), FontAttributes = FontAttributes.Bold },
                                new Label { Text = AppEnvironment.T("project.language_label") },
                                _languagePicker
                            }
                        }
                    },
                    new Border
                    {
                        StrokeThickness = 1,
                        Padding = 16,
                        Content = new VerticalStackLayout
                        {
                            Spacing = 10,
                            Children =
                            {
                                new Label { Text = AppEnvironment.T("project.section_editor"), FontAttributes = FontAttributes.Bold },
                                new Label { Text = AppEnvironment.T("project.name_label") },
                                _projectNameEntry,
                                new Label { Text = AppEnvironment.T("project.description_label") },
                                _projectDescriptionEntry,
                                new Label { Text = AppEnvironment.T("project.path_label") },
                                _projectFolderLabel,
                                new HorizontalStackLayout
                                {
                                    Spacing = 12,
                                    Children = { newProjectButton, saveProjectButton, importButton, reloadButton }
                                }
                            }
                        }
                    },
                    new Border
                    {
                        StrokeThickness = 1,
                        Padding = 16,
                        Content = new VerticalStackLayout
                        {
                            Spacing = 10,
                            Children =
                            {
                                new Label { Text = AppEnvironment.T("project.section_browser"), FontAttributes = FontAttributes.Bold },
                                _projectsView
                            }
                        }
                    },
                    new Label { Text = AppEnvironment.T("project.status_label"), FontAttributes = FontAttributes.Bold },
                    _projectStatusLabel
                }
            }
        };
    }

    private void ConfigureLanguagePicker()
    {
        var languages = AppEnvironment.I18n.GetAvailableLanguages().OrderBy(x => x).ToList();
        _languagePicker.ItemsSource = languages;
        _languagePicker.SelectedItem = AppEnvironment.I18n.CurrentLanguageCode;
        _languagePicker.SelectedIndexChanged -= OnLanguageChanged;
        _languagePicker.SelectedIndexChanged += OnLanguageChanged;
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        if (_languagePicker.SelectedItem is not string selectedLanguage || string.IsNullOrWhiteSpace(selectedLanguage))
        {
            return;
        }

        AppEnvironment.SetPreferredLanguage(selectedLanguage);
        BuildScreen();
    }

    private void CreateProject()
    {
        _workspace.NewProject(_projectNameEntry.Text ?? string.Empty, _projectDescriptionEntry.Text ?? string.Empty);
        _projectFolderLabel.Text = ProjectManager.ProjectsRoot;
        _projectStatusLabel.Text = _workspace.StatusMessage;
    }

    private async System.Threading.Tasks.Task SaveProjectAsync()
    {
        _workspace.CurrentProject.Name = _projectNameEntry.Text ?? string.Empty;
        _workspace.CurrentProject.Description = _projectDescriptionEntry.Text ?? string.Empty;

        if (_workspace.SaveProject(replaceExisting: false, out var duplicateFolder))
        {
            ReloadProjects();
            _projectFolderLabel.Text = _workspace.CurrentProjectFolder;
            _projectStatusLabel.Text = _workspace.StatusMessage;
            return;
        }

        if (!string.IsNullOrWhiteSpace(duplicateFolder))
        {
            var shouldReplace = await DisplayAlert(
                AppEnvironment.T("project.replace_title"),
                AppEnvironment.T("project.replace_message"),
                AppEnvironment.T("common.yes"),
                AppEnvironment.T("common.no"));

            if (shouldReplace && _workspace.SaveProject(replaceExisting: true, out _))
            {
                ReloadProjects();
                _projectFolderLabel.Text = _workspace.CurrentProjectFolder;
                _projectStatusLabel.Text = _workspace.StatusMessage;
            }

            return;
        }

        _projectStatusLabel.Text = _workspace.StatusMessage;
    }

    private async System.Threading.Tasks.Task ImportProjectAsync()
    {
        var pickResult = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = AppEnvironment.T("project.import_manifest")
        });

        if (pickResult is null || string.IsNullOrWhiteSpace(pickResult.FullPath))
        {
            return;
        }

        if (_workspace.ImportProject(pickResult.FullPath, replaceExisting: false, out var duplicateFolder))
        {
            ReloadProjects();
            _projectFolderLabel.Text = _workspace.CurrentProjectFolder;
            _projectStatusLabel.Text = _workspace.StatusMessage;
            return;
        }

        if (!string.IsNullOrWhiteSpace(duplicateFolder))
        {
            var shouldReplace = await DisplayAlert(
                AppEnvironment.T("project.replace_title"),
                AppEnvironment.T("project.replace_message"),
                AppEnvironment.T("common.yes"),
                AppEnvironment.T("common.no"));

            if (shouldReplace && _workspace.ImportProject(pickResult.FullPath, replaceExisting: true, out _))
            {
                ReloadProjects();
                _projectFolderLabel.Text = _workspace.CurrentProjectFolder;
                _projectStatusLabel.Text = _workspace.StatusMessage;
            }

            return;
        }

        _projectStatusLabel.Text = _workspace.StatusMessage;
    }

    private void LoadProject(ProjectSummary projectSummary)
    {
        if (_workspace.LoadProject(projectSummary.FolderPath))
        {
            _projectNameEntry.Text = _workspace.CurrentProject.Name;
            _projectDescriptionEntry.Text = _workspace.CurrentProject.Description;
            _projectFolderLabel.Text = _workspace.CurrentProjectFolder;
            _projectStatusLabel.Text = _workspace.StatusMessage;
        }
    }

    private void ReloadProjects()
    {
        _projects.Clear();
        foreach (var project in _workspace.GetProjects())
        {
            _projects.Add(project);
        }

        if (_projects.Count == 0)
        {
            _projectStatusLabel.Text = AppEnvironment.T("project.no_projects");
        }
    }

    private void OpenFolderBrowser()
    {
        try
        {
            System.Diagnostics.ProcessStartInfo psi = new()
            {
                FileName = "open",
                Arguments = _setupPathEntry.Text ?? GetSuggestedParentPath(),
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);
        }
        catch (Exception ex)
        {
            _setupStatusLabel.Text = $"Error: {ex.Message}";
        }
    }

    private System.Threading.Tasks.Task ConfigureRootAsync()
    {
        var selectedParentDirectory = _setupPathEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(selectedParentDirectory))
        {
            _setupStatusLabel.Text = AppEnvironment.T("setup.status_ready");
            return System.Threading.Tasks.Task.CompletedTask;
        }

        if (AppSetupService.ConfigureRootDirectory(selectedParentDirectory, out var rootDirectory, out var errorMessage))
        {
            AppEnvironment.SetRootDirectory(rootDirectory);
            BuildScreen();
            return System.Threading.Tasks.Task.CompletedTask;
        }

        _setupStatusLabel.Text = errorMessage;
        return System.Threading.Tasks.Task.CompletedTask;
    }

    private System.Threading.Tasks.Task RequestPermissionsAsync()
    {
        _setupStatusLabel.Text = AppEnvironment.T("setup.permission_not_needed");
        return System.Threading.Tasks.Task.CompletedTask;
    }

    private static string GetSuggestedParentPath()
    {
        var desktopDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        if (!string.IsNullOrWhiteSpace(desktopDirectory))
        {
            return desktopDirectory;
        }

        var documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        if (!string.IsNullOrWhiteSpace(documentsDirectory))
        {
            return documentsDirectory;
        }

        return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    }
}
