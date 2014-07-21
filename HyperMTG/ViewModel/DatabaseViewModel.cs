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
		private IDBReader dbReader;
		private IDBWriter dbWriter;

		private Dispatcher dispatcher;
		private string info;

		/// <summary>
		/// Single background method
		/// </summary>
		private volatile bool isProcessing;

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
			get { return new RelayCommand(UpdateSetsExecute, CanExecute); }
		}

		public ICommand UpdateCards
		{
			get { return new RelayCommand<IEnumerable<Set>>(UpdateCardsExecute); }
		}

		public ICommand ExportImages
		{
			get { return new RelayCommand<IEnumerable<Card>>(ExprotImagesExecute); }
		}

		public ICommand LoadDB
		{
			get { return new RelayCommand(LoadDBExecute, CanExecute); }
		}

		private void LoadDBExecute()
		{
			if (dbReader != null)
			{
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
			}
			else
			{
				Info = "Assembly missing";
			}
		}

		private bool CanExecute()
		{
			if (dbWriter != null) return true;
			Info = "Assembly missing";
			return false;
		}

		private void UpdateSetsExecute()
		{
			isProcessing = true;
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

				isProcessing = false;

				Info = "Done!";
			}).Start();
		}

		private void UpdateCardsExecute(IEnumerable<Set> sets)
		{
			throw new NotImplementedException();
		}

		private void ExprotImagesExecute(IEnumerable<Card> cards)
		{
			throw new NotImplementedException();
		}
	}

	internal class CheckSetItem : ObservableObject
	{
		private bool isChecked;
		private double max;
		private double prog;

		public CheckSetItem(bool isChecked, Set content)
		{
			IsChecked = isChecked;
			Content = content;
		}

		public double Prog
		{
			get { return prog; }
			set
			{
				prog = value;
				RaisePropertyChanged("Prog");
			}
		}

		public double Max
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