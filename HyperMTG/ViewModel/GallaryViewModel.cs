using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using HyperMTG.Helper;
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
		private ObservableCollection<ExCard> cards;
		private PageSize size;
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
		}

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
			get { return size; }
			set
			{
				size = value;
				RaisePropertyChanged("Size");
			}
		}

		public ObservableCollection<ExCard> Cards
		{
			get { return cards; }
			set
			{
				cards = value;
				RaisePropertyChanged("Cards");
			}
		}
	}

	public class PageSize : ObservableObject
	{
		private double itemHeight;
		private double itemWidth;
		private double pageHeight;
		private double pageWidth;

		public PageSize()
		{
			ItemWidth = 312;
			ItemHeight = 445;
			PageWidth = 936;
			PageHeight = 1335;
		}

		public double ItemWidth
		{
			get { return itemWidth; }
			set
			{
				itemWidth = value;
				RaisePropertyChanged("ItemWidth");
			}
		}

		public double ItemHeight
		{
			get { return itemHeight; }
			set
			{
				itemHeight = value;
				RaisePropertyChanged("ItemHeight");
			}
		}

		public double PageWidth
		{
			get { return pageWidth; }
			set
			{
				pageWidth = value;
				RaisePropertyChanged("PageWidth");
			}
		}

		public double PageHeight
		{
			get { return pageHeight; }
			set
			{
				pageHeight = value;
				RaisePropertyChanged("PageHeight");
			}
		}

		/// <summary>
		/// Set display ratio (0.0 ~ 1.0)
		/// </summary>
		/// <param name="ratio"></param>
		public void SetRatio(double ratio)
		{
			if (ratio < 0 || ratio > 1)
			{
				return;
			}

			ItemHeight *= ratio;
			ItemWidth *= ratio;
			PageHeight *= ratio;
			PageWidth *= ratio;
		}
	}
}