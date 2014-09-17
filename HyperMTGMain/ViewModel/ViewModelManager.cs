namespace HyperMTGMain.ViewModel
{
	public class ViewModelManager
	{
		public static DatabaseViewModel DatabaseViewModel
		{
			get { return DatabaseViewModel.Instance; }
		}

		public static AboutViewModel AboutViewModel
		{
			get { return AboutViewModel.Instance; }
		}

		public static PreferenceViewModel PreferenceViewModel
		{
			get { return PreferenceViewModel.Instance; }
		}

		public static FilterViewModel FilterViewModel
		{
			get { return FilterViewModel.Instance; }
		}

		public static AnalyzerViewModel AnalyzerViewModel
		{
			get { return AnalyzerViewModel.Instance; }
		}

		public static DeckEditorViewModel DeckEditorViewModel
		{
			get { return DeckEditorViewModel.Instance; }
		}

		public static SealedViewModel SealedViewModel
		{
			get { return SealedViewModel.Instance; }
		}

		public static DraftViewModel DraftViewModel
		{
			get { return DraftViewModel.Instance; }
		}

		public static LauncherViewModel LauncherViewModel
		{
			get { return LauncherViewModel.Instance; }
		}
	}
}