using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using HyperMTG.Helper;
using HyperMTG.Model;
using HyperMTG.Properties;
using HyperPlugin;

namespace HyperMTG.ViewModel
{
	public class GallaryViewModel : ObservableObject
	{
		private readonly ICompressor _compressor;
		private readonly IDBReader _dbReader;
		private readonly IDBWriter _dbWriter;
		private readonly IImageParse _imageParse;
		private ObservableCollection<ExCard> _cards;
		private PageSize _size;
		/// <summary>
		///     UI dispatcher(to handle ObservableCollection)
		/// </summary>
		private readonly Dispatcher _dispatcher;

		public GallaryViewModel()
		{
			_dbReader = PluginManager.Instance.GetPlugin<IDBReader>();
			_compressor = PluginManager.Instance.GetPlugin<ICompressor>();
			_dbWriter = PluginManager.Instance.GetPlugin<IDBWriter>();
			_imageParse = PluginManager.Instance.GetPlugin<IImageParse>();
			if (_dbWriter != null && _dbReader != null)
			{
				_dbWriter.Language = Settings.Default.Language;
				_dbReader.Language = Settings.Default.Language;
			}

			_dispatcher = Application.Current.Dispatcher;

			Size = new PageSize();
			Size.SetRatio(0.5);

			_dispatcher.BeginInvoke(new Action(LoadData));

			Instance = this;
		}

		public static GallaryViewModel Instance { get; private set; }

		private void LoadData()
		{
			Cards = new ObservableCollection<ExCard>();
			foreach (var card in _dbReader.LoadCards().Take(10))
			{
				Cards.Add(new ExCard(_compressor, _dbReader, card,_dbWriter, _imageParse));
			}
		}

		public PageSize Size
		{
			get { return _size; }
			set
			{
				_size = value;
				RaisePropertyChanged("Size");
			}
		}

		public ObservableCollection<ExCard> Cards
		{
			get { return _cards; }
			set
			{
				_cards = value;
				RaisePropertyChanged("Cards");
			}
		}
	}
}