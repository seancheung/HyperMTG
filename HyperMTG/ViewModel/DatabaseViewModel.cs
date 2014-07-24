using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
		/// <summary>
		///     Max thread amount for downloading
		/// </summary>
		private const int MaxThread = 10;

		/// <summary>
		///     Thread Lock
		/// </summary>
		private static readonly object Lock = new object();

		private readonly IDBReader _dbReader;
		private readonly IDBWriter _dbWriter;
		private readonly ICompressor _compressor;

		/// <summary>
		///     UI dispatcher(to handle ObservableCollection)
		/// </summary>
		private readonly Dispatcher _dispatcher;

		private ObservableCollection<Card> _cards;
		private int _currentPage;

		private string _info;
		private int _pageSize;

		private volatile int _processCount;
		private int _recordSize;

		/// <summary>
		/// Thread Canceling
		/// </summary>
		private CancellationTokenSource cts = new CancellationTokenSource();

		public DatabaseViewModel()
		{
			Cards = new ObservableCollection<Card>();
			Sets = new ObservableCollection<CheckSetItem>();

			_dbReader = IOHandler.Instance.GetPlugins<IDBReader>().FirstOrDefault();
			_dbWriter = IOHandler.Instance.GetPlugins<IDBWriter>().FirstOrDefault();
			_compressor = IOHandler.Instance.GetPlugins<ICompressor>().FirstOrDefault();


			_dispatcher = Application.Current.Dispatcher;

			RecordSize = 20;
		}

		/// <summary>
		///     Number of records to display each page
		/// </summary>
		public int RecordSize
		{
			get { return _recordSize; }
			set
			{
				_recordSize = value;
				RaisePropertyChanged("RecordSize");
			}
		}

		public int CurrentPage
		{
			get { return _currentPage; }
			private set
			{
				_currentPage = value;
				RaisePropertyChanged("CurrentPage");
			}
		}

		public int PageSize
		{
			get { return _pageSize; }
			set
			{
				_pageSize = value;
				RaisePropertyChanged("PageSize");
			}
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

		#region Command

		public ICommand CancelCommand
		{
			get { return new RelayCommand(CancelExecute, CanExecuteCancel); }
		}

		public ICommand PageCommand
		{
			get { return new RelayCommand<object>(PageExecute, CanExecutePage); }
		}

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
			get { return new RelayCommand(LoadCardsExecute, CanExecuteLoadCards); }
		}

		#endregion

		#region Execute

		private void CancelExecute()
		{
			cts.Cancel();
		}

		private void PageExecute(object next)
		{
			_processCount++;
			var td = new Thread(() =>
			{
				Info = "Loading";
				IEnumerable<Card> cards = _dbReader.LoadCards();
				int count = cards.Count();
				PageSize = count / RecordSize + (count % RecordSize == 0 ? 0 : 1);
				if (Convert.ToBoolean(next))
					CurrentPage++;
				else
					CurrentPage--;

				var result = cards.Take(RecordSize * CurrentPage).Skip(RecordSize * (CurrentPage - 1));
				_dispatcher.Invoke(
					new Action(() => { Cards = new ObservableCollection<Card>(result); }));
				Info = "Done!";
				_processCount--;
			});
			td.Start();
		}

		private void LoadSetsExecute()
		{
			_processCount++;
			var td = new Thread(() =>
			{
				var sets = new ObservableCollection<Set>(_dbReader.LoadSets());
				_dispatcher.Invoke(new Action(() =>
				{
					Sets.Clear();
					foreach (Set set in sets)
					{
						Sets.Add(new CheckSetItem(false, set));
					}
				}));
				_processCount--;
			});
			td.Start();
		}

		private void LoadCardsExecute()
		{
			_processCount++;
			var td = new Thread(() =>
			{
				Info = "Loading";
				IEnumerable<Card> cards = _dbReader.LoadCards();
				int count = cards.Count();
				PageSize = count / RecordSize + (count % RecordSize == 0 ? 0 : 1);
				CurrentPage = 1;

				var result = cards.Take(RecordSize * CurrentPage).Skip(RecordSize * (CurrentPage - 1));
				_dispatcher.Invoke(
					new Action(() => { Cards = new ObservableCollection<Card>(result); }));

				Info = "Done!";

				_processCount--;

			});
			td.Start();
		}

		private void UpdateSetsExecute()
		{
			_processCount++;
			Info = "Waiting...Grabbing Source";
			var td = new Thread(() =>
			{
				IEnumerable<Set> sets = DataParse.Instance.ParSetWithCode();
				IList<Set> enumerable = sets as IList<Set> ?? sets.ToList();
				_dbWriter.Insert(enumerable);
				_dispatcher.Invoke(new Action(() =>
				{
					Sets.Clear();
					foreach (Set set in enumerable)
					{
						Sets.Add(new CheckSetItem(false, set));
					}
				}));

				_processCount--;

				Info = "Done!";
			});
			td.Start();
		}

		private void UpdateCardsExecute()
		{
			foreach (CheckSetItem checkSetItem in Sets)
			{
				if (checkSetItem.IsChecked)
				{
					var td = new Thread(() =>
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

						WaitCallback waitCallback = param =>
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

							//If action is cancelled, break
							for (int i = 0; i < tmpCards.Count && !cts.Token.IsCancellationRequested; i++)
							{

								Info = enumerable.Count + ": " + tmpCards[i].ID;
								bool result = DataParse.Instance.Process(tmpCards[i], LANGUAGE.ChineseSimplified);
								if (!result)
									tmpCards.RemoveAt(i);
								else
								{
									foreach (var id in tmpCards[i].ID.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
									{
										var data = ImageParse.Instance.Download(id);
										if (data != null && _compressor != null)
											lock (Lock)
											{
												_dbWriter.Insert(id, data, _compressor);
											}
									}
									_dispatcher.Invoke(new Action(() => { Cards.Add(tmpCards[i]); }));
								}

								checkSetItem.Prog++;

							}

							if (!cts.Token.IsCancellationRequested)
							{
								lock (Lock)
								{
									//Save Data
									_dbWriter.Insert(tmpCards);
								}
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

						if (!cts.IsCancellationRequested)
						{
							checkSetItem.IsLocal = true;
							checkSetItem.IsChecked = false;
							_dbWriter.Update(checkSetItem.Content);
						}
						checkSetItem.IsProcessing = false;
						Info = "Done!";
						_processCount--;
					});
					td.Start();
				}
			}
		}

		private void ExprotImagesExecute()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region CanExecute

		private bool CanExecuteCancel()
		{
			return _processCount > 0;
		}

		private bool CanExecutePage(object next)
		{
			return _processCount == 0 && (Convert.ToBoolean(next) ? PageSize - CurrentPage >= 1 : CurrentPage >= 2);
		}

		private bool CanExecuteLoadCards()
		{
			return _processCount == 0 && Sets.Any(s => s.IsLocal) && _dbReader != null;
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

		#endregion
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