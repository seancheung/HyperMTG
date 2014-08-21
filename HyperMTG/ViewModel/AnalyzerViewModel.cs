using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using HyperKore.Common;
using HyperKore.Utilities;
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
				RaisePropertyChanged("SpellsCMC");
			}
		}

		public IEnumerable SpellsCMC
		{
			get { return Deck.MainBoard.Where(c => !c.HasType(Type.Land)).GroupBy(c => c.CMC).Select(g => new { CMC = g.Key, Count = g.Count() }); }
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