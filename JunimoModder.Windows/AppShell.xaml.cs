using JunimoModder.Shared.BaseUIs;

namespace JunimoModder.Windows;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Title = AppEnvironment.T("app.title");
	}
}
