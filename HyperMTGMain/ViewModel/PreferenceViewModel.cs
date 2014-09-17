namespace HyperMTGMain.ViewModel
{
	public class PreferenceViewModel
	{
		private static PreferenceViewModel instance;

		private PreferenceViewModel()
		{
		}

		internal static PreferenceViewModel Instance
		{
			get { return instance ?? (instance = new PreferenceViewModel()); }
		}
	}
}