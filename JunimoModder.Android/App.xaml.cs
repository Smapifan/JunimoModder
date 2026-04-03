namespace JunimoModder.Android;

public partial class App : Application
{
	public App()
	{
		JunimoModder.Shared.BaseUIs.AppBootstrapper.Initialize();
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}