using System.Collections.ObjectModel;
using HyperKore.Common;

namespace HyperMTG.ViewModel
{
	internal class DeckBuiderViewModel
	{
		/// <summary>
		///     Initializes a new instance of the DeckBuiderViewModel class.
		/// </summary>
		public DeckBuiderViewModel()
		{
			Deck = new Deck();
			Cards = new ObservableCollection<Card>();
		}

		public Deck Deck { get; set; }

		public ObservableCollection<Card> Cards { get; private set; }
	}
}