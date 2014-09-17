namespace HyperMTGMain.View
{
	public class ViewManager
	{
		private static SealedWindow sealedWindow;
		private static DraftWindow draftWindow;
		private static AboutWindow aboutWindow;
		private static AnalyzerWindow analyzerWindow;
		private static DatabaseWindow databaseWindow;
		private static DeckEditorWindow deckEditorWindow;
		private static FilterWindow filterWindow;
		private static PreferenceWindow preferenceWindow;
		private static Launcher launcher;

		public static SealedWindow SealedWindow
		{
			get { return sealedWindow ?? (sealedWindow = new SealedWindow()); }
		}

		public static DraftWindow DraftWindow
		{
			get { return draftWindow ?? (draftWindow = new DraftWindow()); }
		}

		public static AboutWindow AboutWindow
		{
			get { return aboutWindow ?? (aboutWindow = new AboutWindow()); }
		}

		public static AnalyzerWindow AnalyzerWindow
		{
			get { return analyzerWindow ?? (analyzerWindow = new AnalyzerWindow()); }
		}

		public static DatabaseWindow DatabaseWindow
		{
			get { return databaseWindow ?? (databaseWindow = new DatabaseWindow()); }
		}

		public static DeckEditorWindow DeckEditorWindow
		{
			get { return deckEditorWindow ?? (deckEditorWindow = new DeckEditorWindow()); }
		}

		public static FilterWindow FilterWindow
		{
			get { return filterWindow ?? (filterWindow = new FilterWindow()); }
		}

		public static PreferenceWindow PreferenceWindow
		{
			get { return preferenceWindow ?? (preferenceWindow = new PreferenceWindow()); }
		}

		public static Launcher Launcher
		{
			get { return launcher ?? (launcher = new Launcher()); }
		}
	}
}