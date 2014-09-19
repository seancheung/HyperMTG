using HyperKore.Common;
using HyperMTGMain.Helper;
using HyperMTGMain.Properties;

namespace HyperMTGMain.Model
{
	public class ImgCard : ObservableClass
	{
		private Card _card;

		public ImgCard(Card card)
		{
			_card = card;
		}

		public ImgCard()
		{
		}

		public Card Card
		{
			get { return _card; }
			set
			{
				_card = value;
				OnPropertyChanged("Image");
				OnPropertyChanged("Card");
			}
		}

		public byte[] Image
		{
			get
			{
				if (!PluginFactory.ComponentsAvailable || Card == null)
				{
					return null;
				}

				byte[] data = PluginFactory.DbReader.LoadFile(Card.ID, PluginFactory.Compressor);
				if (data != null)
					return data;
				data = UpdateData(Card);
				return data;
			}
		}

		private byte[] UpdateData(Card card)
		{
			byte[] data = PluginFactory.ImageParse.Download(card, Settings.Default.Language);
			if (data != null)
				PluginFactory.DbWriter.Insert(card.ID, data, PluginFactory.Compressor);
			return data;
		}
	}
}