namespace HyperMTGMain.ViewModel
{
	public class SealedViewModel
	{
		private static SealedViewModel instance;

		private SealedViewModel()
		{
		}

		internal static SealedViewModel Instance
		{
			get { return instance ?? (instance = new SealedViewModel()); }
		}
	}
}