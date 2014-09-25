using System.Windows.Input;
using HyperMTGMain.Helper;
using HyperMTGMain.View;

namespace HyperMTGMain.ViewModel
{
	public class LauncherViewModel : ObservableClass
	{
		private static LauncherViewModel _instance;

		private LauncherViewModel()
		{
		}

		internal static LauncherViewModel Instance
		{
			get { return _instance ?? (_instance = new LauncherViewModel()); }
		}

		public ICommand OpenAboutCommand
		{
			get { return new RelayCommand(() => ViewManager.AboutWindow.ShowDialog()); }
		}

		public ICommand OpenAnalyzerCommand
		{
			get { return new RelayCommand(() => ViewManager.AnalyzerWindow.ShowDialog()); }
		}

		public ICommand OpenDatabaseCommand
		{
			get { return new RelayCommand(() => ViewManager.DatabaseWindow.ShowDialog()); }
		}

		public ICommand OpenDeckEditorCommand
		{
			get { return new RelayCommand(() => ViewManager.DeckEditorWindow.ShowDialog()); }
		}

		public ICommand OpenDraftCommand
		{
			get { return new RelayCommand(() => ViewManager.RoomListWindow.ShowDialog()); }
		}

		public ICommand OpenFilterCommand
		{
			get { return new RelayCommand(() => ViewManager.FilterWindow.ShowDialog()); }
		}

		public ICommand OpenPreferenceCommand
		{
			get { return new RelayCommand(() => ViewManager.PreferenceWindow.ShowDialog()); }
		}

		public ICommand OpenSealedCommand
		{
			get { return new RelayCommand(() => ViewManager.SealedWindow.ShowDialog()); }
		}

		public ICommand OpenLoginCommand
		{
			get { return new RelayCommand(() => ViewManager.LoginWindow.ShowDialog()); }
		}
	}
}