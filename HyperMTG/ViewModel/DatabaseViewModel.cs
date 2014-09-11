using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using HyperKore.Common;
using HyperKore.Logger;
using HyperMTG.Helper;
using HyperMTG.Model;
using HyperMTG.Properties;
using HyperPlugin;

namespace HyperMTG.ViewModel
{
	internal class DatabaseViewModel : ObservableObject
	{
		/// <summary>
		///     Max thread amount for downloading
		/// </summary>
		private const int MaxThread = 10;

		private readonly ICompressor _compressor;

		/// <summary>
		///     Thread Canceling
		/// </summary>
		private readonly CancellationTokenSource _cts = new CancellationTokenSource();

		private readonly IDataParse _dataParse;
		private readonly IDBReader _dbReader;
		private readonly IDBWriter _dbWriter;

		/// <summary>
		///     UI dispatcher(to handle ObservableCollection)
		/// </summary>
		private readonly Dispatcher _dispatcher;

		private readonly IImageParse _imageParse;

		private List<Card> _cards;
		private int _currentPage;
		private bool _doImage;

		private string _info;
		private int _pageSize;

		private volatile int _processCount;
		private int _recordSize;
		private List<CheckSetItem> _sets;

		public DatabaseViewModel()
		{
			Cards = new List<Card>();
			Sets = new List<CheckSetItem>();

			try
			{
				_dbReader = PluginManager.Instance.GetPlugin<IDBReader>();
				_dbWriter = PluginManager.Instance.GetPlugin<IDBWriter>();
				_compressor = PluginManager.Instance.GetPlugin<ICompressor>();
				_dataParse = PluginManager.Instance.GetPlugin<IDataParse>();
				_imageParse = PluginManager.Instance.GetPlugin<IImageParse>();
			}
			catch (Exception ex)
			{
				Logger.Log(ex, typeof (DatabaseViewModel));
				throw;
			}

			if (_dbWriter != null && _dbReader != null)
			{
				_dbWriter.Language = Settings.Default.Language;
				_dbReader.Language = Settings.Default.Language;
			}

			_dispatcher = Application.Current.Dispatcher;

			RecordSize = 20;

			if (LoadSetsCommand.CanExecute(null))
			{
				LoadSetsCommand.Execute(null);
			}
		}

		public bool SaveImage
		{
			get { return _doImage; }
			set
			{
				_doImage = value;
				RaisePropertyChanged("SaveImage");
			}
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

		public List<Card> Cards
		{
			get { return _cards; }
			set
			{
				_cards = value;
				RaisePropertyChanged("Cards");
			}
		}

		public List<CheckSetItem> Sets
		{
			get { return _sets; }
			set
			{
				_sets = value;
				RaisePropertyChanged("Sets");
			}
		}

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

		public ICommand TestCommand
		{
			get { return new RelayCommand<string>(TestExecute, CanExecuteTest); }
		}

		#endregion

		#region Execute

		private void TestExecute(string id)
		{
			_dataParse.Process(new Card {ID = id, Set = "DarkAscen", SetCode = "DKA"}, Language.ChineseSimplified);
		}

		private void CancelExecute()
		{
			_cts.Cancel();
		}

		private void PageExecute(object next)
		{
			_processCount++;
			Info = "Loading";
			List<Card> result = new List<Card>();

			Task task = new Task(() =>
			{
				IEnumerable<Card> cards = _dbReader.LoadCards();
				int count = cards.Count();
				PageSize = count/RecordSize + (count%RecordSize == 0 ? 0 : 1);
				if (Convert.ToBoolean(next))
					CurrentPage++;
				else
					CurrentPage--;
				try
				{
					result = cards.Take(RecordSize*CurrentPage).Skip(RecordSize*(CurrentPage - 1)).ToList();
				}
				catch (Exception ex)
				{
					Logger.Log(ex, typeof (DatabaseViewModel));
					throw;
				}
			});

			task.Start();
			task.ContinueWith(t =>
			{
				Cards = result;
				_processCount--;
				Info = "Done!";
			});
		}

		private void LoadSetsExecute()
		{
			_processCount++;

			List<CheckSetItem> result = new List<CheckSetItem>();

			Task task = new Task(() =>
			{
				IEnumerable<Set> sets;
				try
				{
					sets = _dbReader.LoadSets();
				}
				catch (Exception ex)
				{
					Logger.Log(ex, typeof (DatabaseViewModel), _dbReader);
					throw;
				}
				result.AddRange(sets.Select(set => new CheckSetItem(false, set)));
			});
			task.Start();
			task.ContinueWith(t =>
			{
				Sets = result;
				_processCount--;
			});
		}

		private void LoadCardsExecute()
		{
			_processCount++;
			Info = "Loading";
			List<Card> result = new List<Card>();

			Task task = new Task(() =>
			{
				IEnumerable<Card> cards;
				try
				{
					cards = _dbReader.LoadCards();
				}
				catch (Exception ex)
				{
					Logger.Log(ex, typeof (DatabaseViewModel), _dbReader);
					throw;
				}

				int count = cards.Count();
				PageSize = count/RecordSize + (count%RecordSize == 0 ? 0 : 1);
				CurrentPage = 1;
				try
				{
					result = cards.Take(RecordSize*CurrentPage).Skip(RecordSize*(CurrentPage - 1)).ToList();
				}
				catch (Exception ex)
				{
					Logger.Log(ex, typeof (DatabaseViewModel));
					throw;
				}
			});

			task.Start();
			task.ContinueWith(t =>
			{
				Cards = result;
				_processCount--;
				Info = "Done!";
			});
		}

		private void UpdateSetsExecute()
		{
			_processCount++;
			Info = "Waiting...Grabbing Source";
			List<CheckSetItem> result = new List<CheckSetItem>();

			Task task = new Task(() =>
			{
				IEnumerable<Set> sets;
				try
				{
					sets = _dataParse.ParseSet();
				}
				catch (Exception ex)
				{
					Logger.Log(ex, typeof (DatabaseViewModel), _dataParse);
					throw;
				}

				try
				{
					_dbWriter.Insert(sets);
				}
				catch (Exception ex)
				{
					Logger.Log(ex, typeof (DatabaseViewModel), _dbWriter);
					throw;
				}

				Parallel.ForEach(sets, set => result.Add(new CheckSetItem(false, set)));
			});

			task.Start();
			task.ContinueWith(t =>
			{
				Sets = result;
				_processCount--;
				Info = "Done!";
			});
		}

		private void UpdateCardsExecute()
		{
			foreach (CheckSetItem checkSetItem in Sets.Where(s => s.IsChecked))
			{
				List<Card> cards = new List<Card>();
				Task task = new Task(() =>
				{
					_processCount++;
					checkSetItem.IsProcessing = true;
					Info = "Preparing for " + checkSetItem.Content.FullName;


					try
					{
						cards.AddRange(_dataParse.Process(checkSetItem.Content, Settings.Default.Language));
						Cards = cards;
					}
					catch (Exception ex)
					{
						Logger.Log(ex, typeof (DatabaseViewModel), _dataParse, checkSetItem.Content, Settings.Default.Language);
						throw;
					}

					checkSetItem.Max = cards.Count();
					checkSetItem.Prog = 0;

					if (SaveImage)
					{
						Parallel.ForEach(cards, card =>
						{
							if (!_cts.Token.IsCancellationRequested)
							{
								Info = string.Format("Downloading image{0}: {1}", card.Set, card.ID);

								try
								{
									byte[] data = _imageParse.Download(card, Settings.Default.Language);
									if (data != null && _compressor != null)
										_dbWriter.Insert(card.ID, data, _compressor);
								}
								catch (Exception ex)
								{
									Logger.Log(ex, typeof (DatabaseViewModel), _imageParse, _compressor, _dbWriter, Settings.Default.Language,
										card.ID);
									throw;
								}

								checkSetItem.Prog++;
							}
						});
					}
				});

				task.ContinueWith(t =>
				{
					if (t.IsCompleted)
					{
						//Save Data
						try
						{
							_dbWriter.Insert(cards);
						}
						catch (Exception ex)
						{
							Logger.Log(ex, typeof (DatabaseViewModel), _dbWriter);
							throw;
						}

						checkSetItem.IsLocal = true;
						checkSetItem.IsChecked = false;
						_dbWriter.Update(checkSetItem.Content);
					}

					checkSetItem.IsProcessing = false;
					Info = "Done!";
					_processCount--;
				});

				task.Start();
			}
		}

		private void ExprotImagesExecute()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region CanExecute

		private bool CanExecuteTest(string id)
		{
			return !string.IsNullOrWhiteSpace(id) && _processCount == 0;
		}

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
			if (_dbWriter == null || _imageParse == null || _dataParse == null || _compressor == null)
			{
				Info = "Assembly missing";
				return false;
			}
			return _processCount == 0;
		}

		private bool CanExecuteRead()
		{
			if (_dbReader != null) return _processCount == 0;
			Info = "Assembly missing";
			return false;
		}

		#endregion
	}
}