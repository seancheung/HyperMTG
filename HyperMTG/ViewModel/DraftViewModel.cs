using System;
using System.Collections;
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
	public class DraftViewModel : ObservableObject
	{
		private readonly ICompressor _compressor;
		private readonly IDBReader _dbReader;
		private readonly IDBWriter _dbWriter;
		private readonly Dispatcher _dispatcher;
		private readonly IImageParse _imageParse;
		private readonly List<ObservableCollection<ExCard>> boosters;
		private readonly int limitTime = 30;
		private readonly double originalCheckSize = 40;
		private readonly DispatcherTimer timer;
		private double _ratio;
		private PageSize _size;
		private ObservableCollection<ExCard> currentBooster;
		private List<ObservableCollection<ExCard>> currentBoosters;
		private bool goRight;
		private ObservableCollection<ExCard> hand;
		private Set[] packs;
		private int playerAmount;
		private List<Set> setSource;
		private int timerTick;

		public DraftViewModel()
		{
			_dbReader = PluginManager.Instance.GetPlugin<IDBReader>();
			_compressor = PluginManager.Instance.GetPlugin<ICompressor>();
			_dbWriter = PluginManager.Instance.GetPlugin<IDBWriter>();
			_imageParse = PluginManager.Instance.GetPlugin<IImageParse>();
			_dispatcher = Application.Current.Dispatcher;
			if (_dbWriter != null && _dbReader != null)
			{
				_dbWriter.Language = Settings.Default.Language;
				_dbReader.Language = Settings.Default.Language;
				SetSource = _dbReader.LoadSets().Where(s => s.Local).ToList();
			}
			timer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 1)};
			timer.Tick += delegate { TimerTick++; };
			boosters = new List<ObservableCollection<ExCard>>();
			Hand = new ObservableCollection<ExCard>();
			Packs = new Set[3];
			Size = new PageSize();
			Ratio = 0.5;
			playerAmount = 2;
			Instance = this;
		}

		public static DraftViewModel Instance { get; private set; }

		public int TimerTick
		{
			get { return timerTick; }
			set
			{
				timerTick = value;
				RaisePropertyChanged("TimerTick");
				CheckTime();
			}
		}

		public ObservableCollection<ExCard> CurrentBooster
		{
			get { return currentBooster; }
			set
			{
				currentBooster = value;
				RaisePropertyChanged("CurrentBooster");
			}
		}

		public ObservableCollection<ExCard> Hand
		{
			get { return hand; }
			set
			{
				hand = value;
				RaisePropertyChanged("Hand");
			}
		}

		public Set[] Packs
		{
			get { return packs; }
			set
			{
				packs = value;
				RaisePropertyChanged("Packs");
			}
		}

		public List<Set> SetSource
		{
			get { return setSource; }
			set
			{
				setSource = value;
				RaisePropertyChanged("SetSource");
			}
		}

		public int PlayerAmount
		{
			get { return playerAmount; }
			set
			{
				playerAmount = value;
				RaisePropertyChanged("PlayerAmount");
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

		#region Command

		public ICommand PickCardCommand
		{
			get { return new RelayCommand<ExCard>(PickCardExecute, CanExecutePick); }
		}

		public ICommand StartCommand
		{
			get { return new RelayCommand(StartExecute, CanExecuteStart); }
		}

		public ICommand SyncCommand
		{
			get { return new RelayCommand(SyncExecute, CanExecuteSync); }
		}

		#endregion

		#region Execute

		public void PickCardExecute(ExCard exCard)
		{
			if (currentBoosters == null)
			{
				currentBoosters = boosters.Take(playerAmount).ToList();
				boosters.RemoveRange(0, playerAmount);
				goRight = !goRight;
			}

			Random ran = new Random();

			foreach (ObservableCollection<ExCard> pack in currentBoosters)
			{
				if (pack != CurrentBooster)
				{
					pack.RemoveAt(ran.Next(0, pack.Count));
				}
			}

			Hand.Add(exCard);
			CurrentBooster.Remove(exCard);

			if (goRight)
			{
				ShiftRight(currentBoosters);
			}
			else
			{
				ShiftLeft(currentBoosters);
			}

			if (!currentBoosters.All(b => b.Any()) && boosters.Any())
			{
				currentBoosters = boosters.Take(playerAmount).ToList();
				boosters.RemoveRange(0, playerAmount);
				goRight = !goRight;
			}

			CurrentBooster = currentBoosters.First();
			TimerTick = 0;
		}

		public void SyncExecute()
		{
			DeckBuiderViewModel.Instance.Deck.MainBoard.Clear();
			DeckBuiderViewModel.Instance.Deck.SideBoard.Clear();

			foreach (Card card in Hand.Select(c => c.Card))
			{
				DeckBuiderViewModel.Instance.Deck.MainBoard.Add(card);
			}
		}

		public void StartExecute()
		{
			IEnumerable<Card> db = _dbReader.LoadCards();
			boosters.Clear();

			Task task = new Task(() =>
			{
				foreach (Set pack in Packs)
				{
					for (int i = 0; i < PlayerAmount; i++)
					{
						List<Card> result = new List<Card>();
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

						ObservableCollection<ExCard> booster = new ObservableCollection<ExCard>();
						foreach (Card card in result)
						{
							booster.Add(new ExCard(_compressor, _dbReader, card, _dbWriter, _imageParse));
						}
						_dispatcher.BeginInvoke(new Action(() => boosters.Add(booster)));
					}
				}
			});
			task.Start();
			task.ContinueWith(t =>
			{
				CurrentBooster = boosters.FirstOrDefault();
				TimerTick = 0;
				timer.Start();
			});
		}

		#endregion

		#region CanExecute

		public bool CanExecutePick(ExCard exCard)
		{
			return CurrentBooster != null;
		}

		public bool CanExecuteStart()
		{
			return _dbReader != null && _dbWriter != null && Packs != null && Packs.All(p => p != null);
		}

		public bool CanExecuteSync()
		{
			return Hand.Count >= 45 && DeckBuiderViewModel.Instance != null;
		}

		#endregion

		private void CheckTime()
		{
			if (TimerTick >= limitTime)
			{
				if (CurrentBooster.Any() && CanExecutePick(CurrentBooster.First()))
				{
					PickCardExecute(CurrentBooster.First());
				}
			}
		}

		private void ShiftRight(IList list, int k = 1)
		{
			if (list == null)
			{
				return;
			}
			int size = list.Count - 1;
			Reverse(list, 0, size - k);
			Reverse(list, size - k + 1, size);
			Reverse(list, 0, size);
		}

		private void ShiftLeft(IList list, int k = 1)
		{
			if (list == null)
			{
				return;
			}
			int size = list.Count - 1;
			Reverse(list, 0, k);
			Reverse(list, k + 1, size);
			Reverse(list, 0, size);
		}

		private void Reverse(IList list, int left, int right)
		{
			while (left <= right)
			{
				object l = list[left];
				object r = list[right];
				list.Insert(left, r);
				list.RemoveAt(left + 1);
				list.Insert(right, l);
				list.RemoveAt(right + 1);

				left++;
				right--;
			}
		}
	}
}