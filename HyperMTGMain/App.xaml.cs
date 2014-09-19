using System.Windows;
using HyperMTGMain.Helper;
using HyperMTGMain.Properties;
using HyperMTGMain.View;

namespace HyperMTGMain
{
	public partial class App
	{
		private void App_OnStartup(object sender, StartupEventArgs e)
		{
			PluginFactory.SetLanguage(Settings.Default.Language);
			ViewManager.Launcher.Show();
		}
	}
}