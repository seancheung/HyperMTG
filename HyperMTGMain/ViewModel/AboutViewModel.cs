namespace HyperMTGMain.ViewModel
{
	public class AboutViewModel
	{
		private static AboutViewModel instance;

		private AboutViewModel()
		{
		}

		internal static AboutViewModel Instance
		{
			get { return instance ?? (instance = new AboutViewModel()); }
		}
	}
}