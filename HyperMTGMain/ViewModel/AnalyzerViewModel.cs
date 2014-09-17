namespace HyperMTGMain.ViewModel
{
	public class AnalyzerViewModel
	{
		private static AnalyzerViewModel _instance;

		private AnalyzerViewModel()
		{
		}

		internal static AnalyzerViewModel Instance
		{
			get { return _instance ?? (_instance = new AnalyzerViewModel()); }
		}
	}
}