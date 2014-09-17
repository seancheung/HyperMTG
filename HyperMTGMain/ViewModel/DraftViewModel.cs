namespace HyperMTGMain.ViewModel
{
	public class DraftViewModel
	{
		private static DraftViewModel instance;

		private DraftViewModel()
		{
		}

		internal static DraftViewModel Instance
		{
			get { return instance ?? (instance = new DraftViewModel()); }
		}
	}
}