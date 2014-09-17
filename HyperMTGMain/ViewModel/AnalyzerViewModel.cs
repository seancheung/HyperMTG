namespace HyperMTGMain.ViewModel
{
	public class AnalyzerViewModel
	{
		private static AnalyzerViewModel instance;

		private AnalyzerViewModel()
		{
		}

		internal static AnalyzerViewModel Instance
		{
			get { return instance ?? (instance = new AnalyzerViewModel()); }
		}
	}
}