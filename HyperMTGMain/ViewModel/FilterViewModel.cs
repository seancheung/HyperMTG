namespace HyperMTGMain.ViewModel
{
	public class FilterViewModel
	{
		private static FilterViewModel _instance;

		private FilterViewModel()
		{
		}

		internal static FilterViewModel Instance
		{
			get { return _instance ?? (_instance = new FilterViewModel()); }
		}
	}
}