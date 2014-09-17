namespace HyperMTGMain.ViewModel
{
	public class AboutViewModel
	{
		private static AboutViewModel _instance;

		private AboutViewModel()
		{
		}

		internal static AboutViewModel Instance
		{
			get { return _instance ?? (_instance = new AboutViewModel()); }
		}
	}
}