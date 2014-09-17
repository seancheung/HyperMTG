namespace HyperMTGMain.ViewModel
{
	public class DatabaseViewModel
	{
		private static DatabaseViewModel instance;

		private DatabaseViewModel()
		{
		}

		internal static DatabaseViewModel Instance
		{
			get { return instance ?? (instance = new DatabaseViewModel()); }
		}
	}
}