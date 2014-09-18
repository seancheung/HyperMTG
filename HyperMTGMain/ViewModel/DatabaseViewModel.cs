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
		private bool _downloadImage;
		private string _info;
		private List<ProgressCheck> _progressChecks;

		internal static DatabaseViewModel Instance
		{
			get { return _instance ?? (_instance = new DatabaseViewModel()); }
		}

		public bool DownloadImage
		{
			get { return _downloadImage; }
			set
			{
				_downloadImage = value;
				OnPropertyChanged("DownloadImage");
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

		public string Info
		{
			get { return _info; }
			set
			{
				_info = value;
				OnPropertyChanged("Info");
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

		private bool CanCancel()
		{
			return TaskManager.Count > 0 && !_cancellationTokenSource.IsCancellationRequested;
		}

		private void Cancel()
		{
			_cancellationTokenSource.Cancel();
		}

		private bool CanUpdateSets()
		{
			return TaskManager.Count <= 0 && PluginFactory.ComponentsAvailable;
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
			_cancellationTokenSource = new CancellationTokenSource();
			CancellationToken token = _cancellationTokenSource.Token;

			foreach (ProgressCheck check in ProgressChecks.Where(p => p.IsChecked))
			{
				#region Data Task

				Task<IEnumerable<Card>> datTask = new Task<IEnumerable<Card>>(() =>
				{
					TaskManager.Count++;
					check.IsProcessing = true;

					try
					{
						token.ThrowIfCancellationRequested();
						return PluginFactory.DataParse.Process(check.Content, Settings.Default.Language);
					}
					catch (Exception ex)
					{
						Logger.Log(ex, this);
						throw;
					}
				}, token);

				datTask.ContinueWith(state =>
				{
					if (state.IsCompleted)
					{
						PluginFactory.DbWriter.Insert(datTask.Result);
					}
					if (!DownloadImage)
					{
						check.IsLocal = true;
						check.IsChecked = false;
						PluginFactory.DbWriter.Update(check.Content);
						check.IsProcessing = false;
					}
					TaskManager.Count--;
				}, token);

				datTask.Start();

				#endregion

				#region Image Task

				Task imgTask = new Task(() =>
				{
					TaskManager.Count++;
					check.Max = datTask.Result.Count();
					check.Prog = 0;

					Parallel.ForEach(datTask.Result, card =>
					{
						token.ThrowIfCancellationRequested();

						try
						{
							byte[] data = PluginFactory.ImageParse.Download(card, Settings.Default.Language);
							if (data != null)
							{
								PluginFactory.DbWriter.Insert(card.ID, data, PluginFactory.Compressor);
							}
							check.Prog++;
						}
						catch (Exception ex)
						{
							Logger.Log(ex, this);
							throw;
						}
					});
				}, token);

				imgTask.ContinueWith(state =>
				{
					if (state.IsCompleted)
					{
						check.IsLocal = true;
						check.IsChecked = false;
						PluginFactory.DbWriter.Update(check.Content);
						check.IsProcessing = false;
					}
					TaskManager.Count--;
				}, token);

				if (DownloadImage)
				{
					imgTask.Start();
				}

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
				PluginFactory.SetLanguage(Language.English);
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