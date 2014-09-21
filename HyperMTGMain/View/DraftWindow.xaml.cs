using System;
using HyperMTGMain.ViewModel;

namespace HyperMTGMain.View
{
	public partial class DraftWindow
	{
		public DraftWindow()
		{
			InitializeComponent();
		}

		private void DraftWindow_OnClosed(object sender, EventArgs e)
		{
			ViewModelManager.DraftViewModel.CloseHost();
		}
	}
}