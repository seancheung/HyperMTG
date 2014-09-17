namespace HyperMTGMain.ViewModel
{
	public class DeckEditorViewModel
	{
		private static DeckEditorViewModel _instance;

		private DeckEditorViewModel()
		{
		}

		internal static DeckEditorViewModel Instance
		{
			get { return _instance ?? (_instance = new DeckEditorViewModel()); }
		}
	}
}