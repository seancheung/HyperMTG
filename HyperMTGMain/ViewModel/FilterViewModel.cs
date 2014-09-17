namespace HyperMTGMain.ViewModel
{
	public class FilterViewModel
	{
		private static FilterViewModel instance;

		private FilterViewModel()
		{
		}

		internal static FilterViewModel Instance
		{
			get { return instance ?? (instance = new FilterViewModel()); }
		}
	}
}