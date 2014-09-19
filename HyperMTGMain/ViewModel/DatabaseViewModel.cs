using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using HyperKore.Common;
using HyperKore.Logger;
using HyperMTGMain.Helper;
using HyperMTGMain.Model;
using HyperMTGMain.Properties;

namespace HyperMTGMain.ViewModel
{
	public class DatabaseViewModel : ObservableClass
	{
		private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();


		private DatabaseViewModel()
		{
		}

		#region Notifiable Prop

		private static DatabaseViewModel _instance;
		private bool _isOverwrite;
		private List<ProgressCheck> _progressChecks;

		internal static DatabaseViewModel Instance
		{
			get { return _instance ?? (_instance = new DatabaseViewModel()); }
		}

		public bool IsOverwrite
		{
			get { return _isOverwrite; }
			set
			{
				_isOverwrite = value;
				OnPropertyChanged("IsOverwrite");
			}
		}

		public List<ProgressCheck> ProgressChecks
		{
			get { return _progressChecks; }
			set
			{
				_progressChecks = value;
				OnPropertyChanged("ProgressChecks");
			}
		}

		#endregion

		#region Commands

		public ICommand CancelCommand
		{
			get { return new RelayCommand(Cancel, CanCancel); }
		}

		public ICommand UpdateSetsCommand
		{
			get { return new RelayCommand(UpdateSets, CanUpdateSets); }
		}

		public ICommand UpdateCardsCommand
		{
			get { return new RelayCommand(UpdateCards, CanUpdateCards); }
		}

		public ICommand UpdateImageCommand
		{
			get { return new RelayCommand(UpdateImages, CanUpdateImages); }
		}

		private bool CanCancel()
		{
			return TaskManager.Count > 0 && !_cancellationTokenSource.IsCancellationRequested;
		}

		private void Cancel()
		{
			_cancellationTokenSource.Cancel(true);
		}

		private bool CanUpdateSets()
		{
			return TaskManager.Count <= 0 && PluginFactory.ComponentsAvailable && ProgressChecks != null &&
			       !ProgressChecks.Any(p => p.IsChecked);
		}

		private void UpdateSets()
		{
			Task<IEnumerable<Set>> task = new Task<IEnumerable<Set>>(() =>
			{
				TaskManager.Count++;
				try
				{
					return PluginFactory.DataParse.ParseSet();
				}
				catch (Exception ex)
				{
					Logger.Log(ex, this);
					throw;
				}
			});
			task.Start();
			task.ContinueWith(state =>
			{
				if (state.IsCompleted)
				{
					PluginFactory.DbWriter.Insert(task.Result);
					LoadSets();
				}
				TaskManager.Count--;
			});
		}

		private bool CanUpdateCards()
		{
			return TaskManager.Count <= 0 && PluginFactory.ComponentsAvailable && ProgressChecks != null &&
			       ProgressChecks.Any(p => p.IsChecked);
		}

		private void UpdateCards()
		{
			foreach (ProgressCheck check in ProgressChecks.Where(p => p.IsChecked))
			{
				#region Data Task

				Task<IEnumerable<Card>> task = new Task<IEnumerable<Card>>(() =>
				{
					TaskManager.Count++;
					check.IsProcessing = true;
					ViewModelManager.MessageViewModel.Inform("Updating {0} started", check.Content.SetName);

					try
					{
						return PluginFactory.DataParse.Process(check.Content, Settings.Default.Language);
					}
					catch (Exception ex)
					{
						Logger.Log(ex, this);
						throw;
					}
				});

				task.ContinueWith(state =>
				{
					if (state.IsCompleted)
					{
						if (task.Result.Any())
						{
							PluginFactory.DbWriter.Insert(task.Result);
							check.IsLocal = true;
							check.IsChecked = false;
							PluginFactory.DbWriter.Update(check.Content);
							ViewModelManager.MessageViewModel.Inform("Updating {0} finished", check.Content.SetName);
						}
						else
						{
							ViewModelManager.MessageViewModel.Inform("{0} not found on server", check.Content.SetName);
						}
					}
					check.IsProcessing = false;
					LoadSets();
					TaskManager.Count--;
				});

				task.Start();

				#endregion
			}
		}

		private bool CanUpdateImages()
		{
			return TaskManager.Count <= 0 && PluginFactory.ComponentsAvailable && ProgressChecks != null &&
			       ProgressChecks.Any(p => p.IsChecked) &&
			       ProgressChecks.Where(p => p.IsChecked).All(p => p.IsLocal);
		}

		private void UpdateImages()
		{
			_cancellationTokenSource = new CancellationTokenSource();
			CancellationToken token = _cancellationTokenSource.Token;
			token.Register(() => ViewModelManager.MessageViewModel.Inform("Cenceling"));
			ParallelOptions parallelOptions = new ParallelOptions
			{
				CancellationToken = token,
				MaxDegreeOfParallelism = Environment.ProcessorCount
			};

			IEnumerable<Card> db = PluginFactory.DbReader.LoadCards();

			foreach (ProgressCheck check in ProgressChecks.Where(p => p.IsChecked))
			{
				#region Image Task

				Task task = new Task(() =>
				{
					TaskManager.Count++;
					IEnumerable<Card> allCards = db.Where(c => c.SetCode == check.Content.SetCode);
					IEnumerable<Card> prcCards = PluginFactory.DbReader.CheckFiles(allCards);
					check.IsProcessing = true;
					check.Max = allCards.Count();
					check.Prog = allCards.Count() - prcCards.Count();

					try
					{
						foreach (Card prcCard in prcCards)
						{
							try
							{
								ViewModelManager.MessageViewModel.Inform("{0}:{1}", prcCard.SetCode, prcCard.Name);
								byte[] data = PluginFactory.ImageParse.Download(prcCard, Settings.Default.Language);
								if (data != null)
								{
									PluginFactory.DbWriter.Insert(prcCard.ID, data, PluginFactory.Compressor);
								}
								check.Prog++;
							}
							catch (Exception ex)
							{
								Logger.Log(ex, this);
								throw;
							}
							token.ThrowIfCancellationRequested();
						}
						//Parallel.ForEach(prcCards, parallelOptions, card =>
						//{
						//	try
						//	{
						//		ViewModelManager.MessageViewModel.Inform("{0}: {1}", card.SetCode, card.Name);
						//		byte[] data = PluginFactory.ImageParse.Download(card, Settings.Default.Language);
						//		if (data != null)
						//		{
						//			PluginFactory.DbWriter.Insert(card.ID, data, PluginFactory.Compressor);
						//		}
						//		check.Prog++;
						//	}
						//	catch (Exception ex)
						//	{
						//		Logger.Log(ex, this);
						//		throw;
						//	}

						//	parallelOptions.CancellationToken.ThrowIfCancellationRequested();
						//});

						ViewModelManager.MessageViewModel.Inform("Done");
					}
					catch (OperationCanceledException)
					{
						ViewModelManager.MessageViewModel.Inform("Canceled");
					}
				});

				task.ContinueWith(state =>
				{
					check.IsProcessing = false;
					check.IsChecked = false;
					TaskManager.Count--;
				});

				task.Start();

				#endregion
			}
		}

		#endregion

		public void LoadSets()
		{
			if (!PluginFactory.ComponentsAvailable)
			{
				return;
			}
			try
			{
				IEnumerable<Set> sets = PluginFactory.DbReader.LoadSets();
				ProgressChecks = new List<ProgressCheck>(sets.Select(p => new ProgressCheck(false, p)));
			}
			catch (Exception ex)
			{
				Logger.Log(ex, this);
				throw;
			}
		}
	}
}