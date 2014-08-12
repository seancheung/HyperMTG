using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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
		private readonly IDeckReader[] _deckReaders;
		private readonly IDeckWriter[] _deckWriters;

		/// <summary>
		///     UI dispatcher(to handle ObservableCollection)
		/// </summary>
		private readonly Dispatcher _dispatcher;

		private readonly IImageParse _imageParse;

		private ExCard _card;

		private ObservableCollection<Card> _cards;
		private Deck _deck;
		private string _info;
		private string input;

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

		public string Input
		{
			get { return input; }
			set
			{
				input = value;
				RaisePropertyChanged("Input");
			}
		}

		#region Command

		public ICommand ExportImageDocCommand
		{
			get { return new RelayCommand(ExportImagesDocExecute, CanExecuteExportImageDoc); }
		}

		public ICommand CopyImageCommand
		{
			get { return new RelayCommand(CopyImageExecute, CanExecuteCopyImage); }
		}

		public ICommand ClearInputCommand
		{
			get { return new RelayCommand(ClearInputExecute, CanExecuteClearInput); }
		}

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

		private void ExportImagesDocExecute()
		{
			var document = new FlowDocument();

			foreach (Card card in Deck.MainBoard)
			{
				var exCard = new ExCard(_compressor, _dbReader, card, _dbWriter, _imageParse);
				byte[] img = exCard.Image;
				if (img != null)
				{
					//document.Blocks.Add(new Paragraph(new Run(card.Name)));
					document.Blocks.Add(new BlockUIContainer(new Image {Source = img.ToBitmapImage(), Width = 312, Height = 445}));
				}
			}
			foreach (Card card in Deck.SideBoard)
			{
				var exCard = new ExCard(_compressor, _dbReader, card, _dbWriter, _imageParse);
				byte[] img = exCard.Image;
				if (img != null)
				{
					//document.Blocks.Add(new Paragraph(new Run(card.Name)));
					document.Blocks.Add(new BlockUIContainer(new Image {Source = img.ToBitmapImage(), Width = 312, Height = 445}));
				}
			}

			using (var fs = new FileStream(DateTime.Now.ToFileTime() + ".rtf", FileMode.Create))
			{
				var sw = new StreamWriter(fs);
				sw.Write(document.ToRTF());
				sw.Flush();
			}
		}

		private void CopyImageExecute()
		{
			Clipboard.SetImage(SelectedCard.Image.ToBitmapImage());
		}

		private void ClearInputExecute()
		{
			Input = null;
		}

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

				result = result.Where(c => string.CompareOrdinal(c.CMC, FilterViewModel.Instance.Cost.ToString()) >= 0);

				if (!string.IsNullOrWhiteSpace(Input))
				{
					if (Regex.IsMatch(Input, @"@"))
					{
						foreach (string exp in Regex.Split(Input, @"&"))
						{
							string[] cons = Regex.Split(Input, @"@");
							if (cons.Length == 2)
							{
								switch (cons[0].ToLower())
								{
									case "n":
										result = result.Where(c => Regex.IsMatch(c.Name, cons[1], RegexOptions.IgnoreCase));
										break;
									case "t":
										result = result.Where(c => Regex.IsMatch(c.Type, cons[1], RegexOptions.IgnoreCase));
										break;
									case "x":
										result = result.Where(c => Regex.IsMatch(c.Text, cons[1], RegexOptions.IgnoreCase));
										break;
									case "i":
										result = result.Where(c => Regex.IsMatch(c.ID, cons[1], RegexOptions.IgnoreCase));
										break;
								}
							}
						}
					}
					else
					{
						result = result.Where(c => Regex.IsMatch(c.Name, Input, RegexOptions.IgnoreCase));
					}
				}

				_dispatcher.BeginInvoke(new Action(() => { Cards = new ObservableCollection<Card>(result); }));
			}).Start();
		}

		private void NewDeckExecute()
		{
			Deck = new Deck();
		}

		private void OpenDeckExecute()
		{
			var dlg = new OpenFileDialog();

			dlg.Filter =
				_deckReaders.Aggregate("",
					(current, deckWriter) =>
						current + string.Format("|{0}(*.{1})|*.{2}", deckWriter.DeckType, deckWriter.FileExt, deckWriter.FileExt))
					.Remove(0, 1);
			dlg.RestoreDirectory = true;

			if (dlg.ShowDialog() == true)
			{
				IDeckReader deckReader = _deckReaders[dlg.FilterIndex - 1];
				using (var fs = (FileStream) dlg.OpenFile())
				{
					Deck = deckReader.Read(fs, Cards);
				}
			}
		}

		private void SaveDeckExecute()
		{
			var dlg = new SaveFileDialog();

			dlg.Filter =
				_deckWriters.Aggregate("",
					(current, deckWriter) =>
						current + string.Format("|{0}(*.{1})|*.{2}", deckWriter.DeckType, deckWriter.FileExt, deckWriter.FileExt))
					.Remove(0, 1);
			dlg.RestoreDirectory = true;

			if (dlg.ShowDialog() == true)
			{
				IDeckWriter deckWriter = _deckWriters[dlg.FilterIndex - 1];
				using (var fs = (FileStream) dlg.OpenFile())
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

		private bool CanExecuteExportImageDoc()
		{
			return Deck.MainBoard.Count + Deck.SideBoard.Count > 0 && _compressor != null && _dbReader != null &&
			       _dbWriter != null && _imageParse != null;
		}

		private bool CanExecuteCopyImage()
		{
			return SelectedCard.Image != null;
		}

		private bool CanExecuteClearInput()
		{
			return !string.IsNullOrWhiteSpace(Input);
		}

		private bool CanExecuteFilter()
		{
			return _dbReader != null;
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