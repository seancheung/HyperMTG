using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using HyperKore.Common;
using HyperKore.IO;
using HyperMTG.Helper;

namespace HyperMTG.ViewModel
{
	internal class DatabaseViewModel
	{
		/// <summary>
		/// InfoBar message
		/// </summary>
		public volatile string Info;

		/// <summary>
		/// ProgressBar value
		/// </summary>
		public volatile int Progress;

		private IDBReader dbReader;
		private IDBWriter dbWriter;

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
		}

		public ObservableCollection<Card> Cards { get; set; }

		public ObservableCollection<CheckSetItem> Sets { get; set; }

		public ICommand UpdateSets
		{
			get { return new RelayCommand(UpdateSetsExecute); }
		}

		public ICommand UpdateCards
		{
			get { return new RelayCommand<IEnumerable<Set>>(UpdateCardsExecute); }
		}

		public ICommand ExportImages
		{
			get { return new RelayCommand<IEnumerable<Card>>(ExprotImagesExecute); }
		}

		private void LoadDB()
		{
			if (dbReader != null)
			{
				Cards = new ObservableCollection<Card>(dbReader.LoadCards());
				ObservableCollection<Set> sets = new ObservableCollection<Set>(dbReader.LoadSets());
				foreach (Set set in sets)
				{
					Sets.Add(new CheckSetItem(false, set));
				}
			}
			else
			{
				Info = "Assembly missing";
			}
		}

		private void UpdateSetsExecute()
		{
			throw new NotImplementedException();
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

		public CheckSetItem(bool isChecked, Set content)
		{
			IsChecked = isChecked;
			Content = content;
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