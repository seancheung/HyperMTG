namespace HyperMTG.ViewModel
{
	using HyperKore.Common;
	using System.Collections.Generic;

	internal class DeckBuiderViewModel
	{
		public Deck Deck { get; set; }

		public IEnumerable<Card> Cards { get; private set; }

		/// <summary>
		/// Initializes a new instance of the DeckBuiderViewModel class.
		/// </summary>
		public DeckBuiderViewModel()
		{
			Deck = new Deck();
			Cards = null;
		}
	}
}