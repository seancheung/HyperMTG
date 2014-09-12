using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using HyperKore.Common;
using HyperKore.Utilities;
using HyperMTG.Helper;
using HyperMTG.Model;
using HyperMTG.Properties;
using HyperPlugin;

namespace HyperMTG.ViewModel
{
	public class SealedViewModel : ObservableObject
	{
		private readonly ICompressor _compressor;
		private readonly IDBReader _dbReader;
		private readonly IDBWriter _dbWriter;
		private readonly Dispatcher _dispatcher;
		private readonly IImageParse _imageParse;
		private readonly double originalCheckSize = 40;
		private ObservableCollection<ExCard> _cards;
		private Set[] _packs;
		private double _ratio;
		private List<Set> _setSource;
		private PageSize _size;
		private bool isCheckVisible;
		private string[] sorts;

		public SealedViewModel()
		{
			_dbReader = PluginManager.Instance.GetPlugin<IDBReader>();
			_compressor = PluginManager.Instance.GetPlugin<ICompressor>();
			_dbWriter = PluginManager.Instance.GetPlugin<IDBWriter>();
			_imageParse = PluginManager.Instance.GetPlugin<IImageParse>();
			_dispatcher = Application.Current.Dispatcher;
			Size = new PageSize();
			Packs = new Set[6];
			Sorts = new string[2];
			Cards = new ObservableCollection<ExCard>();

			if (_dbWriter != null && _dbReader != null)
			{
				_dbWriter.Language = Settings.Default.Language;
				_dbReader.Language = Settings.Default.Language;
				SortSource = new List<string>
				{
					"",
					"CMC",
					"Rarity",
					"Name",
					"Color"
				};
				SetSource = _dbReader.LoadSets().Where(s => s.Local).ToList();
				Ratio = 0.5;
			}

			Instance = this;
		}

		public Set[] Packs
		{
			get { return _packs; }
			set
			{
				_packs = value;
				RaisePropertyChanged("Packs");
			}
		}

		public ObservableCollection<ExCard> Cards
		{
			get { return _cards; }
			set
			{
				_cards = value;
				RaisePropertyChanged("Cards");
			}
		}

		public PageSize Size
		{
			get { return _size; }
			set
			{
				_size = value;
				RaisePropertyChanged("Size");
			}
		}

		public double CheckSize { get; set; }

		public bool IsCheckVisible
		{
			get { return isCheckVisible; }
			set
			{
				isCheckVisible = value;
				RaisePropertyChanged("IsCheckVisible");
			}
		}

		public double Ratio
		{
			get { return _ratio; }
			set
			{
				_ratio = value;
				RaisePropertyChanged("Ratio");
				Size.SetRatio(value);
				CheckSize = originalCheckSize*value;
				RaisePropertyChanged("CheckSize");
			}
		}

		public List<Set> SetSource
		{
			get { return _setSource; }
			set
			{
				_setSource = value;
				RaisePropertyChanged("SetSource");
			}
		}

		public List<string> SortSource { get; set; }

		public string[] Sorts
		{
			get { return sorts; }
			set
			{
				sorts = value;
				RaisePropertyChanged("Sorts");
			}
		}

		#region Command

		public ICommand GenerateCommand
		{
			get { return new RelayCommand(GenerateExecute, CanExecuteGenerate); }
		}

		public ICommand SortCommand
		{
			get { return new RelayCommand(SortExecute, CanExecuteSort); }
		}

		public ICommand SyncCommand
		{
			get { return new RelayCommand(SyncExecute, CanExecuteSync); }
		}

		public static SealedViewModel Instance { get; private set; }

		#endregion

		#region Execute

		public void GenerateExecute()
		{
			List<Card> result = new List<Card>();
			IEnumerable<Card> db = _dbReader.LoadCards();
			Cards = new ObservableCollection<ExCard>();

			Task task = new Task(() =>
			{
				foreach (Set pack in Packs)
				{
					IEnumerable<Card> cards = db.Where(c => c.SetCode == pack.SetCode);

					result.AddRange(cards.Where(c => c.GetRarity() == Rarity.Mythic || c.GetRarity() == Rarity.Rare)
						.ToArray()
						.GetRandoms());
					result.AddRange(cards.Where(c => c.GetRarity() == Rarity.Uncommon)
						.ToArray()
						.GetRandoms(3));
					if (cards.Any(c => c.IsBasicLand()))
					{
						result.AddRange(cards.Where(c => c.GetRarity() == Rarity.Common)
							.ToArray()
							.GetRandoms(10));
						result.AddRange(cards.Where(c => c.IsBasicLand())
							.ToArray()
							.GetRandoms());
					}
					else
					{
						result.AddRange(cards.Where(c => c.GetRarity() == Rarity.Common)
							.ToArray()
							.GetRandoms(11));
					}
				}
			});
			task.Start();
			task.ContinueWith(t =>
			{
				foreach (Card card in result)
				{
					_dispatcher.BeginInvoke(
						new Action(() => { Cards.Add(new ExCard(_compressor, _dbReader, card, _dbWriter, _imageParse)); }));
				}
			});
		}

		public void SortExecute()
		{
			if (Cards != null)
			{
				foreach (string sort in Sorts.Reverse())
				{
					if (!string.IsNullOrWhiteSpace(sort))
					{
						switch (sort)
						{
							case "CMC":
								Cards = new ObservableCollection<ExCard>(Cards.OrderBy(c => c.Card.ParsedCMC()));
								break;
							case "Rarity":
								Cards = new ObservableCollection<ExCard>(Cards.OrderByDescending(c => c.Card.GetRarity()));
								break;
							case "Name":
								Cards = new ObservableCollection<ExCard>(Cards.OrderBy(c => c.Card.Name));
								break;
							case "Color":
								Cards = new ObservableCollection<ExCard>(Cards.OrderByDescending(c => c.Card.GetColors().First()));
								break;
						}
					}
				}
			}
		}

		public void SyncExecute()
		{
			DeckBuiderViewModel.Instance.Deck.MainBoard.Clear();
			DeckBuiderViewModel.Instance.Deck.SideBoard.Clear();

			foreach (Card card in Cards.Where(c => c.IsChecked).Select(c => c.Card))
			{
				DeckBuiderViewModel.Instance.Deck.MainBoard.Add(card);
			}
		}

		#endregion

		#region CanExecute

		public bool CanExecuteGenerate()
		{
			return _dbReader != null && _dbWriter != null && Packs != null && Packs.All(p => p != null);
		}

		public bool CanExecuteSort()
		{
			return Sorts.Any(s => !string.IsNullOrWhiteSpace(s)) && Cards.Any();
		}

		public bool CanExecuteSync()
		{
			return Cards.Any(c => c.IsChecked) && DeckBuiderViewModel.Instance != null;
		}

		#endregion
	}
}