namespace HyperMTGMain.ViewModel
{
	public class MessageViewModel
	{
		private static MessageViewModel _instance;

		private MessageViewModel()
		{
		}

		internal static MessageViewModel Instance
		{
			get { return _instance ?? (_instance = new MessageViewModel()); }
		}
	}
}