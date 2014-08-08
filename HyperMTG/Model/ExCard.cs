using HyperKore.Common;
using HyperMTG.Helper;
using HyperMTG.Properties;
using HyperPlugin;

namespace HyperMTG.Model
{
	public class ExCard : ObservableObject
	{
		private readonly ICompressor _compressor;
		private readonly IDBReader _dbReader;
		private readonly IDBWriter _dbWriter;
		private readonly IImageParse _imageParse;
		private Card _card;

		public ExCard(IDBReader dbReader, ICompressor compressor, IDBWriter dbWriter, IImageParse imageParse)
		{
			_dbReader = dbReader;
			_compressor = compressor;
			_dbWriter = dbWriter;
			_imageParse = imageParse;
		}

		public ExCard(ICompressor compressor, IDBReader dbReader, Card card, IDBWriter dbWriter, IImageParse imageParse)
		{
			_compressor = compressor;
			_dbReader = dbReader;
			_dbWriter = dbWriter;
			_imageParse = imageParse;
			_card = card;
		}

		public ExCard()
		{
		}

		public Card Card
		{
			get { return _card; }
			set
			{
				_card = value;
				RaisePropertyChanged("Image");
				RaisePropertyChanged("Card");
			}
		}

		public byte[] Image
		{
			get
			{
				if (Card == null || _compressor == null || _dbReader == null)
					return null;
				byte[] data = _dbReader.LoadFile(Card.ID, _compressor);
				if (data != null)
					return data;

				if (_dbWriter == null || _imageParse == null) return null;

				lock (Card)
				{
					data = _imageParse.Download(Card, Settings.Default.Language);
					if (data != null)
						_dbWriter.Insert(Card.ID, data, _compressor);
				}

				return data;
			}
		}
	}
}