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
using Type = HyperKore.Common.Type;

namespace HyperMTG.ViewModel
{
	public class SealedViewModel : ObservableObject
	{
		private readonly ICompressor _compressor;
		private readonly IDBReader _dbReader;
		private readonly IDBWriter _dbWriter;
		private readonly Dispatcher _dispatcher;
		private readonly IImageParse _imageParse;
		private ObservableCollection<ExCard> _cards;
		private Set[] _packs;
		private double _ratio;
		private List<Set> _sets;
		private PageSize _size;

		public SealedViewModel()
		{
			_dbReader = PluginManager.Instance.GetPlugin<IDBReader>();
			_compressor = PluginManager.Instance.GetPlugin<ICompressor>();
			_dbWriter = PluginManager.Instance.GetPlugin<IDBWriter>();
			_imageParse = PluginManager.Instance.GetPlugin<IImageParse>();
			_dispatcher = Application.Current.Dispatcher;
			Size = new PageSize();
			Packs = new Set[6];

			if (_dbWriter != null && _dbReader != null)
			{
				_dbWriter.Language = Settings.Default.Language;
				_dbReader.Language = Settings.Default.Language;
				Sets = _dbReader.LoadSets().Where(s => s.Local).ToList();
				Ratio = 0.5;
			}
		}

		public ICommand GenerateCommand
		{
			get { return new RelayCommand(GenerateExecute, CanExecuteGenerate); }
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

		public double Ratio
		{
			get { return _ratio; }
			set
			{
				_ratio = value;
				RaisePropertyChanged("Ratio");
				Size.SetRatio(value);
			}
		}

		public List<Set> Sets
		{
			get { return _sets; }
			set
			{
				_sets = value;
				RaisePropertyChanged("Sets");
			}
		}

		public void GenerateExecute()
		{
			var result = new List<Card>();
			IEnumerable<Card> db = _dbReader.LoadCards();
			Cards = new ObservableCollection<ExCard>();

			var task = new Task(() =>
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
					if (cards.Any(c => c.HasType(Type.Land)))
					{
						result.AddRange(cards.Where(c => c.GetRarity() == Rarity.Common)
							.ToArray()
							.GetRandoms(10));
						result.AddRange(cards.Where(c => c.HasType(Type.Land))
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

		public bool CanExecuteGenerate()
		{
			return _dbReader != null && Packs != null && Packs.All(p => p != null);
		}
	}
}