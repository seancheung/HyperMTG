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
		private IDBReader dbReader;
		private IDBWriter dbWriter;

		private volatile bool isProcessing;

		public DatabaseViewModel()
		{
			Cards = new ObservableCollection<Card>();
			Sets = new ObservableCollection<Set>();

			dbReader = IOHandler.Instance.GetPlugins<IDBReader>().FirstOrDefault();
			dbWriter = IOHandler.Instance.GetPlugins<IDBWriter>().FirstOrDefault();
		}

		public ObservableCollection<Card> Cards { get; set; }

		public ObservableCollection<Set> Sets { get; set; }

		public ICommand UpdateSets
		{
			get { return new RelayCommand(UpdateSetsExecute); }
		}

		public ICommand UpdateCards
		{
			get { return new RelayCommand<IEnumerable<Set>>(UpdateCardsExecute); }
		}

		private void LoadDB()
		{
			if (dbReader != null)
			{
				Cards = new ObservableCollection<Card>(dbReader.LoadCards());
				Sets = new ObservableCollection<Set>(dbReader.LoadSets());
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
	}
}