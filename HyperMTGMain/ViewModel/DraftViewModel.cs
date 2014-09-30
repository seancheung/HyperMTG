using HyperMTGMain.Helper;

namespace HyperMTGMain.ViewModel
{
	public class DraftViewModel : ObservableClass
	{
		private static DraftViewModel _instance;

		internal static DraftViewModel Instance
		{
			get { return _instance ?? (_instance = new DraftViewModel()); }
		}
	}
}