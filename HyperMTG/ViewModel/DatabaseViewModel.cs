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
		private const int maxThread = 10;
		private static readonly object _lock = new object();
		private readonly IDBReader dbReader;
		private readonly IDBWriter dbWriter;

		private readonly Dispatcher dispatcher;
		private string info;

		//Max thread amount for downloading

		//private volatile bool isProcessing;
		private volatile int processCount;

		public DatabaseViewModel()
		{
			Cards = new ObservableCollection<Card>();
			Sets = new ObservableCollection<CheckSetItem>();

			dbReader = IOHandler.Instance.GetPlugins<IDBReader>().FirstOrDefault();
			dbWriter = IOHandler.Instance.GetPlugins<IDBWriter>().FirstOrDefault();

			dispatcher = Application.Current.Dispatcher;
		}

		/// <summary>
		/// InfoBar message
		/// </summary>
		public string Info
		{
			get { return info; }
			set
			{
				info = value;
				RaisePropertyChanged("Info");
			}
		}

		public ObservableCollection<Card> Cards { get; set; }

		public ObservableCollection<CheckSetItem> Sets { get; set; }

		public ICommand UpdateSets
		{
			get { return new RelayCommand(UpdateSetsExecute, CanWriteExecute); }
		}

		public ICommand UpdateCards
		{
			get { return new RelayCommand(UpdateCardsExecute, CanWriteExecute); }
		}

		public ICommand ExportImages
		{
			get { return new RelayCommand(ExprotImagesExecute, CanWriteExecute); }
		}

		public ICommand LoadDatabase
		{
			get { return new RelayCommand(LoadDatabaseExecute, CanReadExecute); }
		}

		private void LoadDatabaseExecute()
		{
			if (dbReader != null)
			{
				processCount++;
				Cards = new ObservableCollection<Card>(dbReader.LoadCards());
				ObservableCollection<Set> sets = new ObservableCollection<Set>(dbReader.LoadSets());
				dispatcher.BeginInvoke(new Action(() =>
				{
					Sets.Clear();
					foreach (Set set in sets)
					{
						Sets.Add(new CheckSetItem(false, set));
					}
				}));
				processCount--;
			}
			else
			{
				Info = "Assembly missing";
			}
		}

		private bool CanWriteExecute()
		{
			if (dbWriter != null)
			{
				if (processCount == 0) return true;
				Info = "Busy: " + processCount;
				return false;
			}
			Info = "Assembly missing";
			return false;
		}

		private bool CanReadExecute()
		{
			if (dbReader != null)
			{
				if (processCount == 0) return true;
				Info = "Busy: " + processCount;
				return false;
			}
			Info = "Assembly missing";
			return false;
		}

		private void UpdateSetsExecute()
		{
			processCount++;
			Info = "Waiting...Grabbing Source";
			new Thread(() =>
			{
				IEnumerable<Set> sets = DataParse.Instance.ParSetWithCode();
				IList<Set> enumerable = sets as IList<Set> ?? sets.ToList();
				dbWriter.Insert(enumerable);
				dispatcher.BeginInvoke(new Action(() =>
				{
					Sets.Clear();
					foreach (Set set in enumerable)
					{
						Sets.Add(new CheckSetItem(false, set));
					}
				}));

				processCount--;

				Info = "Done!";
			}).Start();
		}

		private void UpdateCardsExecute()
		{
			//isProcessing = true;
			foreach (CheckSetItem checkSetItem in Sets)
			{
				if (checkSetItem.IsChecked)
				{
					new Thread(() =>
					{
						processCount++;
						checkSetItem.IsProcessing = true;
						Info = "Preparing for " + checkSetItem.Content.FullName;
						IEnumerable<Card> cards = DataParse.Instance.Prepare(checkSetItem.Content);
						List<Card> enumerable = cards as List<Card> ?? cards.ToList();
						checkSetItem.Max = enumerable.Count();
						checkSetItem.Prog = 0;

						//Split the full card list into several parts
						List<List<Card>> cardsThread = new List<List<Card>>();
						for (int i = 0; i < maxThread - 1; i++)
						{
							cardsThread.Add(enumerable.GetRange(enumerable.Count / maxThread * i, enumerable.Count / maxThread));
						}
						cardsThread.Add(enumerable.GetRange(enumerable.Count / maxThread * (maxThread - 1),
							enumerable.Count / maxThread + enumerable.Count % maxThread));

						#region WaitCallback

						WaitCallback waitCallback = delegate(object param)
						{
							object[] parameres = param as object[];
							if (parameres == null || parameres.Length != 2)
								return;
							IList<Card> tmpCards = parameres[0] as IList<Card>;
							if (tmpCards == null)
								return;
							AutoResetEvent waitHandle = parameres[1] as AutoResetEvent;
							if (waitHandle == null)
								return;

							for (int i = 0; i < tmpCards.Count; i++)
							{
								Info = enumerable.Count + ": " + tmpCards[i].ID;
								if (DataParse.Instance.Process(tmpCards[i], LANGUAGE.ChineseSimplified))
									tmpCards.RemoveAt(i);

								checkSetItem.Prog++;
							}

							lock (_lock)
							{
								//Save Data
								dbWriter.Insert(tmpCards);
							}

							//Set the current thread state as finished
							waitHandle.Set();
						};

						#endregion

						WaitHandle[] waitHandles = new WaitHandle[maxThread];

						//Start a thread pool for updating
						for (int i = 0; i < maxThread; i++)
						{
							waitHandles[i] = new AutoResetEvent(false);
							ThreadPool.QueueUserWorkItem(waitCallback, new object[] { cardsThread[i], waitHandles[i] });
						}

						//Wait for all downloading threads to finish
						WaitHandle.WaitAll(waitHandles);

						checkSetItem.IsLocal = true;
						checkSetItem.IsProcessing = false;
						processCount--;
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
		private bool isChecked;
		private int max;
		private int prog;
		private bool isProcessing;

		public CheckSetItem(bool isChecked, Set content)
		{
			IsChecked = isChecked;
			Content = content;
		}

		public bool IsProcessing
		{
			get { return isProcessing; }
			set
			{
				isProcessing = value;
				RaisePropertyChanged("IsProcessing");
			}
		}

		public int Prog
		{
			get { return prog; }
			set
			{
				prog = value;
				RaisePropertyChanged("Prog");
			}
		}

		public int Max
		{
			get { return max; }
			set
			{
				max = value;
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
				RaisePropertyChanged("IsLocal");
			}
		}

		public bool IsChecked
		{
			get { return isChecked; }
			set
			{
				isChecked = value;
				RaisePropertyChanged("IsChecked");
			}
		}
	}
}