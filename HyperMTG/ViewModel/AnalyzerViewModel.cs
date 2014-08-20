using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using HyperKore.Common;
using HyperMTG.Helper;

namespace HyperMTG.ViewModel
{
	internal class AnalyzerViewModel : ObservableObject
	{
		private Deck deck;

		public Deck Deck
		{
			get { return deck; }
			set
			{
				deck = value;
				RaisePropertyChanged("Deck");
				RaisePropertyChanged("ManaSeries");
			}
		}

		public IEnumerable ManaSeries
		{
			get { return Deck.MainBoard.GroupBy(c => c.CMC).Select(g => new { CMC = g.Key, Count = g.Count() }); }
		}

		public ICommand SyncDeckCommand
		{
			get { return new RelayCommand(ExecuteSyncDeck); }
		}

		private void ExecuteSyncDeck()
		{
			Deck = DeckBuiderViewModel.GetCurrentDeck();
		}
	}
}