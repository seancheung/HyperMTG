using System.ComponentModel;
using System.Windows;
using HyperMTGMain.Helper;
using HyperMTGMain.ViewModel;

namespace HyperMTGMain.View
{
	public partial class DatabaseWindow
	{
		public DatabaseWindow()
		{
			InitializeComponent();
		}

		private void DatabaseWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			ViewModelManager.MessageViewModel.Clear();
			ViewModelManager.DatabaseViewModel.LoadSets();
		}

		private void DatabaseWindow_OnClosing(object sender, CancelEventArgs e)
		{
			if (TaskManager.Count > 0)
			{
				ViewModelManager.MessageViewModel.Message("Download not finished");
				e.Cancel = true;
			}
		}
	}
}