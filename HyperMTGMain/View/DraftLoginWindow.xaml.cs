using System.Windows;
using HyperMTGMain.ViewModel;

namespace HyperMTGMain.View
{
	public partial class DraftLoginWindow
	{
		public DraftLoginWindow()
		{
			InitializeComponent();
		}

		private void DraftLoginWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			ViewModelManager.DraftViewModel.LoadSets();
		}
	}
}