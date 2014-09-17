namespace HyperMTGMain.ViewModel
{
	public class DatabaseViewModel
	{
		private static DatabaseViewModel _instance;

		private DatabaseViewModel()
		{
		}

		internal static DatabaseViewModel Instance
		{
			get { return _instance ?? (_instance = new DatabaseViewModel()); }
		}
	}
}