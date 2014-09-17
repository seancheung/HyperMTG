namespace HyperMTGMain.ViewModel
{
	public class PreferenceViewModel
	{
		private static PreferenceViewModel _instance;

		private PreferenceViewModel()
		{
		}

		internal static PreferenceViewModel Instance
		{
			get { return _instance ?? (_instance = new PreferenceViewModel()); }
		}
	}
}