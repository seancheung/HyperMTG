using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using HyperKore.Common;
using HyperMTG.Helper;
using HyperMTG.Properties;
using HyperPlugin;

namespace HyperMTG.ViewModel
{
	internal class DeckBuiderViewModel : ObservableObject
	{
		private readonly ICompressor _compressor;

		private readonly IDBReader _dbReader;
		private readonly IDBWriter _dbWriter;

		/// <summary>
		///     UI dispatcher(to handle ObservableCollection)
		/// </summary>
		private readonly Dispatcher _dispatcher;

		private readonly IImageParse _imageParse;

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
			_imageParse = PluginManager.Instance.GetPlugin<IImageParse>();

			if (_dbWriter != null && _dbReader != null)
			{
				_dbWriter.Language = Settings.Default.Language;
				_dbReader.Language = Settings.Default.Language;
			}

			_dispatcher = Application.Current.Dispatcher;

			if (_dbReader != null) Cards = new ObservableCollection<Card>(_dbReader.LoadCards());
			else Info = "Assemblly Missing";

			SelectedCard = new ExCard(_dbReader, _compressor, _dbWriter, _imageParse);
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

		#region Command

		public ICommand MoveToSideCommand
		{
			get { return new RelayCommand<Card>(MoveToSideExecute, CanExecuteDeleteCardMain); }
		}

		public ICommand MoveToMainCommand
		{
			get { return new RelayCommand<Card>(MoveToMainExecute, CanExecuteDeleteCardSide); }
		}

		public ICommand AddCardMainCommand
		{
			get { return new RelayCommand<Card>(AddCardMainExecute, CanExecuteAddCardMain); }
		}

		public ICommand AddCardSideCommand
		{
			get { return new RelayCommand<Card>(AddCardSideExecute, CanExecuteAddCardSide); }
		}

		public ICommand DeleteCardMainCommand
		{
			get { return new RelayCommand<Card>(DeleteCardMainExecute, CanExecuteDeleteCardMain); }
		}

		public ICommand DeleteCardSideCommand
		{
			get { return new RelayCommand<Card>(DeleteCardSideExecute, CanExecuteDeleteCardSide); }
		}

		#endregion

		#region Execute

		private void AddCardMainExecute(Card card)
		{
			Deck.MainBoard.Add(card);
		}

		private void AddCardSideExecute(Card card)
		{
			Deck.SideBoard.Add(card);
		}

		private void DeleteCardMainExecute(Card card)
		{
			Deck.MainBoard.Remove(card);
		}

		private void DeleteCardSideExecute(Card card)
		{
			Deck.SideBoard.Remove(card);
		}

		private void MoveToSideExecute(Card card)
		{
			Deck.MainBoard.Remove(card);
			Deck.SideBoard.Add(card);
		}

		private void MoveToMainExecute(Card card)
		{
			Deck.SideBoard.Remove(card);
			Deck.MainBoard.Add(card);
		}

		#endregion

		#region CanExecute

		private bool CanExecuteDeleteCardMain(Card card)
		{
			return Deck.MainBoard.Contains(card);
		}

		private bool CanExecuteDeleteCardSide(Card card)
		{
			return Deck.SideBoard.Contains(card);
		}

		private bool CanExecuteAddCardMain(Card card)
		{
			return card != null;
		}

		private bool CanExecuteAddCardSide(Card card)
		{
			return card != null;
		}

		#endregion
	}

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