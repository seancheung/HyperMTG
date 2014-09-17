namespace HyperMTGMain.ViewModel
{
	public class SealedViewModel
	{
		private static SealedViewModel _instance;

		private SealedViewModel()
		{
		}

		internal static SealedViewModel Instance
		{
			get { return _instance ?? (_instance = new SealedViewModel()); }
		}
	}
}