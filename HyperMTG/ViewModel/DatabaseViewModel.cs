using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using HyperKore.Common;
using HyperKore.IO;
using HyperKore.Web;
using HyperMTG.Helper;

namespace HyperMTG.ViewModel
{
	internal class DatabaseViewModel : ObservableObject
	{
		//Max thread amount for downloading
		private const int MaxThread = 10;
		private static readonly object Lock = new object();
		private readonly IDBReader _dbReader;
		private readonly IDBWriter _dbWriter;

		private readonly Dispatcher _dispatcher;
		private string _info;

		private volatile int _processCount;
		private ObservableCollection<Card> _cards;

		public DatabaseViewModel()
		{
			Cards = new ObservableCollection<Card>();
			Sets = new ObservableCollection<CheckSetItem>();

			_dbReader = IOHandler.Instance.GetPlugins<IDBReader>().FirstOrDefault();
			_dbWriter = IOHandler.Instance.GetPlugins<IDBWriter>().FirstOrDefault();

			_dispatcher = Application.Current.Dispatcher;
		}

		/// <summary>
		///     InfoBar message
		/// </summary>
		public string Info
		{
			get { return _info; }
			set
			{
				_info = value;
				RaisePropertyChanged("Info");
			}
		}

		public ObservableCollection<Card> Cards
		{
			get { return _cards; }
			set
			{
				_cards = value;
				RaisePropertyChanged("Cards");
			}
		}

		public ObservableCollection<CheckSetItem> Sets { get; set; }

		public ICommand UpdateSetsCommand
		{
			get { return new RelayCommand(UpdateSetsExecute, CanExecuteWrite); }
		}

		public ICommand UpdateCardsCommand
		{
			get { return new RelayCommand(UpdateCardsExecute, CanExecuteWrite); }
		}

		public ICommand ExportImagesCommand
		{
			get { return new RelayCommand(ExprotImagesExecute, CanExecuteWrite); }
		}

		public ICommand LoadSetsCommand
		{
			get { return new RelayCommand(LoadSetsExecute, CanExecuteRead); }
		}

		public ICommand LoadCardsCommand
		{
			get { return new RelayCommand<Set>(LoadCardsExecute, CanExecuteLoadCards); }
		}

		private void LoadSetsExecute()
		{
			_processCount++;
			var sets = new ObservableCollection<Set>(_dbReader.LoadSets());
			_dispatcher.BeginInvoke(new Action(() =>
			{
				Sets.Clear();
				foreach (Set set in sets)
				{
					Sets.Add(new CheckSetItem(false, set));
				}
			}));
			_processCount--;
		}

		private void LoadCardsExecute(Set set)
		{
			_processCount++;
			new Thread(() =>
			{
				Info = "Loading " + set.FullName;
				IEnumerable<Card> cards = _dbReader.LoadCards();
				_dispatcher.BeginInvoke(
					new Action(() => { Cards = new ObservableCollection<Card>(cards.Where(c => c.SetCode == set.SetCode)); }));

				Info = "Done!";
				_processCount--;
			}).Start();
		}

		private bool CanExecuteLoadCards(Set set)
		{
			return _processCount == 0 && set != null && set.Local && _dbReader != null;
		}

		private bool CanExecuteWrite()
		{
			if (_dbWriter != null)
			{
				return _processCount == 0;
			}
			Info = "Assembly missing";
			return false;
		}

		private bool CanExecuteRead()
		{
			if (_dbReader != null)
			{
				return _processCount == 0;
			}
			Info = "Assembly missing";
			return false;
		}

		private void UpdateSetsExecute()
		{
			_processCount++;
			Info = "Waiting...Grabbing Source";
			new Thread(() =>
			{
				IEnumerable<Set> sets = DataParse.Instance.ParSetWithCode();
				IList<Set> enumerable = sets as IList<Set> ?? sets.ToList();
				_dbWriter.Insert(enumerable);
				_dispatcher.BeginInvoke(new Action(() =>
				{
					Sets.Clear();
					foreach (Set set in enumerable)
					{
						Sets.Add(new CheckSetItem(false, set));
					}
				}));

				_processCount--;

				Info = "Done!";
			}).Start();
		}

		private void UpdateCardsExecute()
		{
			foreach (CheckSetItem checkSetItem in Sets)
			{
				if (checkSetItem.IsChecked)
				{
					new Thread(() =>
					{
						_processCount++;
						checkSetItem.IsProcessing = true;
						Info = "Preparing for " + checkSetItem.Content.FullName;
						IEnumerable<Card> cards = DataParse.Instance.Prepare(checkSetItem.Content);
						List<Card> enumerable = cards as List<Card> ?? cards.ToList();
						checkSetItem.Max = enumerable.Count();
						checkSetItem.Prog = 0;

						//Split the full card list into several parts
						var cardsThread = new List<List<Card>>();
						for (int i = 0; i < MaxThread - 1; i++)
						{
							cardsThread.Add(enumerable.GetRange(enumerable.Count / MaxThread * i, enumerable.Count / MaxThread));
						}
						cardsThread.Add(enumerable.GetRange(enumerable.Count / MaxThread * (MaxThread - 1),
							enumerable.Count / MaxThread + enumerable.Count % MaxThread));

						#region WaitCallback

						WaitCallback waitCallback = delegate(object param)
						{
							var parameres = param as object[];
							if (parameres == null || parameres.Length != 2)
								return;
							var tmpCards = parameres[0] as IList<Card>;
							if (tmpCards == null)
								return;
							var waitHandle = parameres[1] as AutoResetEvent;
							if (waitHandle == null)
								return;

							for (int i = 0; i < tmpCards.Count; i++)
							{
								Info = enumerable.Count + ": " + tmpCards[i].ID;
								bool result = DataParse.Instance.Process(tmpCards[i], LANGUAGE.ChineseSimplified);
								if (!result)
									tmpCards.RemoveAt(i);
								else
									_dispatcher.Invoke(new Action(() => { Cards.Add(tmpCards[i]); }));

								checkSetItem.Prog++;
							}

							lock (Lock)
							{
								//Save Data
								_dbWriter.Insert(tmpCards);
							}

							//Set the current thread state as finished
							waitHandle.Set();
						};

						#endregion

						var waitHandles = new WaitHandle[MaxThread];

						//Start a thread pool for updating
						for (int i = 0; i < MaxThread; i++)
						{
							waitHandles[i] = new AutoResetEvent(false);
							ThreadPool.QueueUserWorkItem(waitCallback, new object[] { cardsThread[i], waitHandles[i] });
						}

						//Wait for all downloading threads to finish
						WaitHandle.WaitAll(waitHandles);

						checkSetItem.IsLocal = true;
						checkSetItem.IsProcessing = false;
						checkSetItem.IsChecked = false;
						_dbWriter.Update(checkSetItem.Content);
						_processCount--;
					}).Start();
				}
			}
		}

		private void ExprotImagesExecute()
		{
			throw new NotImplementedException();
		}
	}

	internal class CheckSetItem : ObservableObject
	{
		private bool _isChecked;
		private bool _isProcessing;
		private int _max;
		private int _prog;

		public CheckSetItem(bool isChecked, Set content)
		{
			IsChecked = isChecked;
			Content = content;
		}

		public bool IsProcessing
		{
			get { return _isProcessing; }
			set
			{
				_isProcessing = value;
				RaisePropertyChanged("IsProcessing");
			}
		}

		public int Prog
		{
			get { return _prog; }
			set
			{
				_prog = value;
				RaisePropertyChanged("Prog");
			}
		}

		public int Max
		{
			get { return _max; }
			set
			{
				_max = value;
				RaisePropertyChanged("Max");
			}
		}

		public Set Content { get; set; }

		public bool IsLocal
		{
			get { return Content.Local; }
			set
			{
				Content.Local = value;
				Content.LastUpdate = DateTime.Now;
				RaisePropertyChanged("IsLocal");
			}
		}

		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				RaisePropertyChanged("IsChecked");
			}
		}
	}
}