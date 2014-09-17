namespace HyperMTGMain.ViewModel
{
	public class DeckEditorViewModel
	{
		private static DeckEditorViewModel instance;

		private DeckEditorViewModel()
		{
		}

		internal static DeckEditorViewModel Instance
		{
			get { return instance ?? (instance = new DeckEditorViewModel()); }
		}
	}
}