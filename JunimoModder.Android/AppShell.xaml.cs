using JunimoModder.Shared.BaseUIs;

namespace JunimoModder.Android;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Title = AppEnvironment.T("app.title");
	}
}
