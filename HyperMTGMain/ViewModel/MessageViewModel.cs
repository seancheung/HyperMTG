using HyperMTGMain.Helper;

namespace HyperMTGMain.ViewModel
{
	public class MessageViewModel:ObservableClass
	{
		private static MessageViewModel _instance;
		private string _info;

		private MessageViewModel()
		{
		}

		internal static MessageViewModel Instance
		{
			get { return _instance ?? (_instance = new MessageViewModel()); }
		}

		public string Info
		{
			get { return _info; }
			set
			{
				_info = value;
				OnPropertyChanged("Info");
			}
		}

		public void ShowMessage(string msg)
		{
			
		}

	}
}