using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using HyperKore.Common;
using HyperMTG.Helper;
using HyperPlugin;

namespace HyperMTG.ViewModel
{
	internal class DeckBuiderViewModel : ObservableObject
	{
		/// <summary>
		///     Thread Lock
		/// </summary>
		private static readonly object Lock = new object();

		private readonly ICompressor _compressor;

		private readonly IDBReader _dbReader;
		private readonly IDBWriter _dbWriter;

		/// <summary>
		///     UI dispatcher(to handle ObservableCollection)
		/// </summary>
		private readonly Dispatcher _dispatcher;

		private ExCard _card;

		private ObservableCollection<Card> _cards;
		private Deck _deck;
		private string _info;

		/// <summary>
		///     Initializes a new instance of the DeckBuiderViewModel class.
		/// </summary>
		public DeckBuiderViewModel()
		{
			Deck = new Deck();
			Cards = new ObservableCollection<Card>();

			_dbReader = PluginManager.Instance.GetPlugin<IDBReader>();
			_dbWriter = PluginManager.Instance.GetPlugin<IDBWriter>();
			_compressor = PluginManager.Instance.GetPlugin<ICompressor>();

			_dispatcher = Application.Current.Dispatcher;

			if (_dbReader != null) Cards = new ObservableCollection<Card>(_dbReader.LoadCards());
			else Info = "Assemblly Missing";

			SelectedCard = new ExCard(_dbReader, _compressor);
		}

		public ExCard SelectedCard
		{
			get { return _card; }
			set
			{
				_card = value;
				RaisePropertyChanged("SelectedCard");
			}
		}

		public Deck Deck
		{
			get { return _deck; }
			set
			{
				_deck = value;
				RaisePropertyChanged("Deck");
			}
		}

		public ObservableCollection<Card> Cards
		{
			get { return _cards; }
			private set
			{
				_cards = value;
				RaisePropertyChanged("Cards");
			}
		}

		public string Info
		{
			get { return _info; }
			set
			{
				_info = value;
				RaisePropertyChanged("Info");
			}
		}
	}

	internal class ExCard : ObservableObject
	{
		private readonly ICompressor _compressor;
		private readonly IDBReader _dbReader;
		private Card _card;

		public ExCard(IDBReader dbReader, ICompressor compressor)
		{
			_dbReader = dbReader;
			_compressor = compressor;
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
				RaisePropertyChanged("ImageA");
				RaisePropertyChanged("ImageB");
				RaisePropertyChanged("zImageA");
				RaisePropertyChanged("zImageB");
				RaisePropertyChanged("Card");
			}
		}

		public byte[] ImageA
		{
			get
			{
				return Card != null && _dbReader != null && _compressor != null ? _dbReader.LoadFile(IDA, _compressor) : null;
			}
		}

		public byte[] ImageB
		{
			get
			{
				return Card != null && _dbReader != null && _compressor != null ? _dbReader.LoadFile(IDB, _compressor) : null;
			}
		}

		public byte[] zImageA
		{
			get
			{
				return Card != null && _dbReader != null && _compressor != null ? _dbReader.LoadFile(zIDA, _compressor) : null;
			}
		}

		public byte[] zImageB
		{
			get
			{
				return Card != null && _dbReader != null && _compressor != null ? _dbReader.LoadFile(zIDB, _compressor) : null;
			}
		}

		public string IDA
		{
			get { return Card != null ? Card.ID.Contains("|") ? Card.ID.Split('|')[0] : Card.ID : null; }
		}

		public string IDB
		{
			get { return Card != null ? Card.ID.Contains("|") ? Card.ID.Split('|')[1] : null : null; }
		}

		public string zIDA
		{
			get { return Card != null && Card.zID != null ? Card.zID.Contains("|") ? Card.zID.Split('|')[0] : Card.zID : null; }
		}

		public string zIDB
		{
			get { return Card != null && Card.zID != null ? Card.zID.Contains("|") ? Card.zID.Split('|')[1] : null : null; }
		}
	}
}