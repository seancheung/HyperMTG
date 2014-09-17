namespace HyperMTGMain.ViewModel
{
	public class LauncherViewModel
	{
		private static LauncherViewModel instance;

		private LauncherViewModel()
		{
		}

		internal static LauncherViewModel Instance
		{
			get { return instance ?? (instance = new LauncherViewModel()); }
		}
	}
}