using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using HyperKore.Common;
using HyperKore.IO;

namespace HyperMTG.ViewModel
{
	internal class DatabaseViewModel
	{
		public ObservableCollection<Card> Cards { get; set; }

		public ObservableCollection<Set> Sets { get; set; }

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

		private void LoadDB()
		{
			if (dbReader != null)
			{
				Cards = new ObservableCollection<Card>(dbReader.LoadCards());
				Sets = new ObservableCollection<Set>(dbReader.LoadSets());
			}
		}

		private void UpdateSets()
		{

		}
	}
}