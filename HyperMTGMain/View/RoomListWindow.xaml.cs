using System;
using HyperMTGMain.ViewModel;

namespace HyperMTGMain.View
{
	public partial class RoomListWindow
	{
		public RoomListWindow()
		{
			InitializeComponent();
		}

		private void RoomListWindow_OnClosed(object sender, EventArgs e)
		{
			ViewModelManager.OnlineViewModel.Close();
		}
	}
}