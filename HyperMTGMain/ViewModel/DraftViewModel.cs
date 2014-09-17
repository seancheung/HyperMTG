namespace HyperMTGMain.ViewModel
{
	public class DraftViewModel
	{
		private static DraftViewModel _instance;

		private DraftViewModel()
		{
		}

		internal static DraftViewModel Instance
		{
			get { return _instance ?? (_instance = new DraftViewModel()); }
		}
	}
}