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
		private static MessageWindow _messageWindow;
		public static Launcher Launcher = new Launcher();
		private static RoomListWindow _roomListWindow;
		private static LoginWindow _loginWindow;

		public static SealedWindow SealedWindow
		{
			get
			{
				if (_sealedWindow == null)
				{
					_sealedWindow = new SealedWindow();
					_sealedWindow.Closed += delegate
					{
						_sealedWindow = null;
						Launcher.Show();
					};
					_sealedWindow.Loaded += delegate { Launcher.Hide(); };
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
					_draftWindow.Closed += delegate
					{
						_draftWindow = null;
						Launcher.Show();
					};
					_draftWindow.Loaded += delegate { Launcher.Hide(); };
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
					_aboutWindow.Closed += delegate
					{
						_aboutWindow = null;
						Launcher.Show();
					};
					_aboutWindow.Loaded += delegate { Launcher.Hide(); };
				}
				return _aboutWindow;
			}
		}

		public static AnalyzerWindow AnalyzerWindow
		{
			get
			{
				if (_analyzerWindow == null)
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
					_databaseWindow.Closed += delegate
					{
						_databaseWindow = null;
						Launcher.Show();
					};
					_databaseWindow.Loaded += delegate { Launcher.Hide(); };
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
					_deckEditorWindow.Closed += delegate
					{
						_deckEditorWindow = null;
						Launcher.Show();
					};
					_deckEditorWindow.Loaded += delegate { Launcher.Hide(); };
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
					_preferenceWindow.Closed += delegate
					{
						_preferenceWindow = null;
						Launcher.Show();
					};
					_preferenceWindow.Loaded += delegate { Launcher.Hide(); };
				}
				return _preferenceWindow;
			}
		}

		public static MessageWindow MessageWindow
		{
			get
			{
				if (_messageWindow == null)
				{
					_messageWindow = new MessageWindow();
					_messageWindow.Closed += delegate { _messageWindow = null; };
				}
				return _messageWindow;
			}
		}

		public static RoomListWindow RoomListWindow
		{
			get
			{
				if (_roomListWindow == null)
				{
					_roomListWindow = new RoomListWindow();
					_roomListWindow.Closed += delegate
					{
						_roomListWindow = null;
						Launcher.Show();
					};
					_roomListWindow.Loaded += delegate
					{
						Launcher.Hide();
						LoginWindow.Close();
					};
				}
				return _roomListWindow;
			}
		}

		public static LoginWindow LoginWindow
		{
			get
			{
				if (_loginWindow == null)
				{
					_loginWindow = new LoginWindow();
					_loginWindow.Closed += delegate { _loginWindow = null; };
				}
				return _loginWindow;
			}
		}
	}
}