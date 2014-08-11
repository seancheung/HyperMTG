using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using HyperKore.Common;
using HyperKore.Utilities;
using HyperMTG.Helper;
using HyperMTG.Model;
using HyperMTG.Properties;
using HyperPlugin;
using Microsoft.Win32;

namespace HyperMTG.ViewModel
{
	internal class DeckBuiderViewModel : ObservableObject
	{
		private readonly ICompressor _compressor;

		private readonly IDBReader _dbReader;
		private readonly IDBWriter _dbWriter;
		private readonly IDeckWriter[] _deckWriters;
		private readonly IDeckReader[] _deckReaders;

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
			IImageParse imageParse = PluginManager.Instance.GetPlugin<IImageParse>();
			_deckWriters = PluginManager.Instance.GetPlugins<IDeckWriter>().ToArray();
			_deckReaders = PluginManager.Instance.GetPlugins<IDeckReader>().ToArray();

			if (_dbWriter != null && _dbReader != null)
			{
				_dbWriter.Language = Settings.Default.Language;
				_dbReader.Language = Settings.Default.Language;
			}

			_dispatcher = Application.Current.Dispatcher;

			if (_dbReader != null) Cards = new ObservableCollection<Card>(_dbReader.LoadCards());
			else Info = "Assemblly Missing";

			SelectedCard = new ExCard(_dbReader, _compressor, _dbWriter, imageParse);
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

		public ICommand FilterCommand
		{
			get { return new RelayCommand(FilterExecute, CanExecuteFilter); }
		}


		public ICommand NewDeckCommand
		{
			get { return new RelayCommand(NewDeckExecute, CanExecuteNewDeck); }
		}

		public ICommand OpenDeckCommand
		{
			get { return new RelayCommand(OpenDeckExecute, CanExecuteOpenDeck); }
		}

		public ICommand SaveDeckCommand
		{
			get { return new RelayCommand(SaveDeckExecute, CanExecuteSaveDeck); }
		}

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

		private void FilterExecute()
		{
			new Thread(() =>
			{
				IEnumerable<Card> result = _dbReader.LoadCards();

				foreach (var checkItem in FilterViewModel.Instance.Sets)
				{
					if (checkItem.IsChecked == true)
					{
						result = result.Where(c => c.SetCode == checkItem.Content.SetCode);
					}
					else if (checkItem.IsChecked == null)
					{
						result = result.Where(c => c.SetCode != checkItem.Content.SetCode);
					}
				}

				foreach (var checkItem in FilterViewModel.Instance.Colors)
				{
					if (checkItem.IsChecked == true)
					{
						result = result.Where(c => c.GetColors().Contains(checkItem.Content));
					}
					else if (checkItem.IsChecked == null)
					{
						result = result.Where(c => !c.GetColors().Contains(checkItem.Content));
					}
				}

				foreach (var checkItem in FilterViewModel.Instance.Types)
				{
					if (checkItem.IsChecked == true)
					{
						result = result.Where(c => c.GetTypes().Contains(checkItem.Content));
					}
					else if (checkItem.IsChecked == null)
					{
						result = result.Where(c => !c.GetTypes().Contains(checkItem.Content));
					}
				}

				//
				result = result.Where(c => string.CompareOrdinal(c.CMC, FilterViewModel.Instance.Cost.ToString()) >= 0);

				_dispatcher.BeginInvoke(new Action(() =>
				{
					Cards = new ObservableCollection<Card>(result);
				}));

			}).Start();
		}

		private void NewDeckExecute()
		{
			Deck = new Deck();
		}

		private void OpenDeckExecute()
		{
			OpenFileDialog dlg = new OpenFileDialog();

			dlg.Filter = _deckReaders.Aggregate("", (current, deckWriter) => current + string.Format("|{0}(*.{1})|*.{2}", deckWriter.DeckType, deckWriter.FileExt, deckWriter.FileExt)).Remove(0, 1);
			dlg.RestoreDirectory = true;

			if (dlg.ShowDialog() == true)
			{
				IDeckReader deckReader = _deckReaders[dlg.FilterIndex - 1];
				using (FileStream fs = (FileStream)dlg.OpenFile())
				{
					Deck = deckReader.Read(fs, Cards);
				}
			}
		}

		private void SaveDeckExecute()
		{
			SaveFileDialog dlg = new SaveFileDialog();

			dlg.Filter = _deckWriters.Aggregate("", (current, deckWriter) => current + string.Format("|{0}(*.{1})|*.{2}", deckWriter.DeckType, deckWriter.FileExt, deckWriter.FileExt)).Remove(0, 1);
			dlg.RestoreDirectory = true;

			if (dlg.ShowDialog() == true)
			{
				IDeckWriter deckWriter = _deckWriters[dlg.FilterIndex - 1];
				using (FileStream fs = (FileStream)dlg.OpenFile())
				{
					deckWriter.Write(Deck, fs);
				}

				Info = "File successfully saved";
			}
		}

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

		private bool CanExecuteFilter()
		{
			return Cards.Count > 0 && _dbReader != null;
		}

		private bool CanExecuteNewDeck()
		{
			return Deck.MainBoard.Count > 0 || Deck.SideBoard.Count > 0;
		}

		private bool CanExecuteOpenDeck()
		{
			return _deckReaders != null && _deckReaders.Length > 0 && Cards.Count > 0;
		}

		private bool CanExecuteSaveDeck()
		{
			return _deckWriters != null && _deckWriters.Length > 0 && (Deck.MainBoard.Count > 0 || Deck.SideBoard.Count > 0);
		}

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
}