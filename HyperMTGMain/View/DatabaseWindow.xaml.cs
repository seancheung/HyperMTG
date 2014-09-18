using System;
using System.Windows;
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
			ViewModelManager.DatabaseViewModel.LoadSets();
		}
	}
}