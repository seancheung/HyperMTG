using System.Collections;
using System.Linq;
using System.Windows.Input;
using HyperKore.Common;
using HyperKore.Utilities;
using HyperMTG.Helper;

namespace HyperMTG.ViewModel
{
	internal class AnalyzerViewModel : ObservableObject
	{
		private Deck _deck;

		public Deck Deck
		{
			get { return _deck; }
			set
			{
				_deck = value;
				RaisePropertyChanged("Deck");
				RaisePropertyChanged("SpellsCMCSource");
				RaisePropertyChanged("LandSource");
				RaisePropertyChanged("TypeSource");
				RaisePropertyChanged("ColorSource");
				RaisePropertyChanged("RaritySource");
				RaisePropertyChanged("SetSource");
				RaisePropertyChanged("ManaProduceSource");
			}
		}

		public IEnumerable SpellsCMCSource
		{
			get
			{
				return
					Deck.MainBoard.Where(c => !c.HasType(Type.Land))
						.GroupBy(c => c.ParsedCMC())
						.Select(g => new {CMC = g.Key, Count = g.Count()})
						.OrderBy(t => t.CMC);
			}
		}

		public IEnumerable LandSource
		{
			get
			{
				return
					Deck.MainBoard.Where(c => c.HasType(Type.Land))
						.GroupBy(c => c.IsBasicLand())
						.Select(g => new {IsBasic = g.Key ? "Basic" : "None Basic", Count = g.Count()});
			}
		}

		public IEnumerable ManaProduceSource
		{
			get
			{
				return
					Deck.MainBoard.Where(c => c.CanProduceMana())
						.SelectMany(c => c.GetTypes())
						.Where(
							t =>
								t == Type.Land || t == Type.Artifact || t == Type.Creature || t == Type.Instant || t == Type.Sorcery ||
								t == Type.Enchantment || t == Type.Planeswalker)
						.GroupBy(t => t)
						.Select(g => new {Type = g.Key, Count = g.Count()}).OrderBy(p => p.Type);
			}
		}

		public IEnumerable TypeSource
		{
			get
			{
				return
					Deck.MainBoard.Where(c => !c.HasType(Type.Land))
						.SelectMany(c => c.GetTypes())
						.Where(
							t =>
								t == Type.Land || t == Type.Artifact || t == Type.Creature || t == Type.Instant || t == Type.Sorcery ||
								t == Type.Enchantment || t == Type.Planeswalker)
						.GroupBy(t => t)
						.Select(g => new {Type = g.Key, Count = g.Count()}).OrderBy(p => p.Type);
			}
		}

		public IEnumerable ColorSource
		{
			get
			{
				return
					Deck.MainBoard.Where(c => !c.HasType(Type.Land)).SelectMany(c => c.GetColors())
						.GroupBy(t => t)
						.Select(g => new {Color = g.Key, Count = g.Count()})
						.OrderBy(p => p.Color);
			}
		}

		public IEnumerable RaritySource
		{
			get
			{
				return
					Deck.MainBoard.Select(c => c.GetRarity())
						.GroupBy(t => t)
						.Select(g => new {Rarity = g.Key, Count = g.Count()})
						.OrderBy(p => p.Rarity);
			}
		}

		public IEnumerable SetSource
		{
			get
			{
				return
					Deck.MainBoard.Select(c => c.Set)
						.GroupBy(t => t)
						.Select(g => new {Set = g.Key, Count = g.Count()})
						.OrderByDescending(p => p.Count);
			}
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