namespace HyperMTGMain.View
{
	public class ViewManager
	{
		private static SealedWindow _sealedWindow;
		private static DraftWindow _draftWindow;
		private static AboutWindow _aboutWindow;
		private static AnalyzerWindow _analyzerWindow;
		private static DatabaseWindow _databaseWindow;
		private static DeckEditorWindow _deckEditorWindow;
		private static FilterWindow _filterWindow;
		private static PreferenceWindow _preferenceWindow;
		private static Launcher _launcher;

		public static SealedWindow SealedWindow
		{
			get
			{
				if (_sealedWindow == null)
				{
					_sealedWindow = new SealedWindow();
					_sealedWindow.Closed += delegate { _sealedWindow = null; };
				}
				return _sealedWindow;
			}
		}

		public static DraftWindow DraftWindow
		{
			get
			{
				if (_draftWindow == null)
				{
					_draftWindow = new DraftWindow();
					_draftWindow.Closed += delegate { _draftWindow = null; };
				}
				return _draftWindow;
			}
		}

		public static AboutWindow AboutWindow
		{
			get
			{
				if (_aboutWindow == null)
				{
					_aboutWindow = new AboutWindow();
					_aboutWindow.Closed += delegate { _aboutWindow = null; };
				}
				return _aboutWindow;
			}
		}

		public static AnalyzerWindow AnalyzerWindow
		{
			get
			{
				if (_aboutWindow == null)
				{
					_analyzerWindow = new AnalyzerWindow();
					_analyzerWindow.Closed += delegate { _analyzerWindow = null; };
				}
				return _analyzerWindow;
			}
		}

		public static DatabaseWindow DatabaseWindow
		{
			get
			{
				if (_databaseWindow == null)
				{
					_databaseWindow = new DatabaseWindow();
					_databaseWindow.Closed += delegate { _databaseWindow = null; };
				}
				return _databaseWindow;
			}
		}

		public static DeckEditorWindow DeckEditorWindow
		{
			get
			{
				if (_deckEditorWindow == null)
				{
					_deckEditorWindow = new DeckEditorWindow();
					_deckEditorWindow.Closed += delegate { _deckEditorWindow = null; };
				}
				return _deckEditorWindow;
			}
		}

		public static FilterWindow FilterWindow
		{
			get
			{
				if (_filterWindow == null)
				{
					_filterWindow = new FilterWindow();
					_filterWindow.Closed += delegate { _filterWindow = null; };
				}
				return _filterWindow;
			}
		}

		public static PreferenceWindow PreferenceWindow
		{
			get
			{
				if (_preferenceWindow == null)
				{
					_preferenceWindow = new PreferenceWindow();
					_preferenceWindow.Closed += delegate { _preferenceWindow = null; };
				}
				return _preferenceWindow;
			}
		}

		public static Launcher Launcher
		{
			get
			{
				if (_launcher == null)
				{
					_launcher = new Launcher();
					_launcher.Closed += delegate { _launcher = null; };
				}
				return _launcher;
			}
		}
	}
}