using System;
using System.Collections.Generic;
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
using HyperKore.Logger;
using HyperKore.Utilities;
using HyperMTG.Helper;
using HyperMTG.Model;
using HyperMTG.Properties;
using HyperPlugin;
using Microsoft.Win32;
using Type = HyperKore.Common.Type;

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

		private List<Card> _cards;
		private static Deck _deck;
		private string _info;
		private string input;

		private volatile int _processCount;

		/// <summary>
		///     Initializes a new instance of the DeckBuiderViewModel class.
		/// </summary>
		public DeckBuiderViewModel()
		{
			Deck = new Deck();
			Cards = new List<Card>();

			try
			{
				_dbReader = PluginManager.Instance.GetPlugin<IDBReader>();
				_dbWriter = PluginManager.Instance.GetPlugin<IDBWriter>();
				_compressor = PluginManager.Instance.GetPlugin<ICompressor>();
				_imageParse = PluginManager.Instance.GetPlugin<IImageParse>();
				_deckWriters = PluginManager.Instance.GetPlugins<IDeckWriter>().ToArray();
				_deckReaders = PluginManager.Instance.GetPlugins<IDeckReader>().ToArray();
			}
			catch (Exception ex)
			{
				Logger.Log(ex, typeof(DatabaseViewModel));
				throw;
			}

			if (_dbWriter != null && _dbReader != null)
			{
				_dbWriter.Language = Settings.Default.Language;
				_dbReader.Language = Settings.Default.Language;
			}

			_dispatcher = Application.Current.Dispatcher;

			if (_dbReader != null)
			{
				try
				{
					Cards = _dbReader.LoadCards().ToList();
				}
				catch (Exception ex)
				{
					Logger.Log(ex, typeof(DatabaseViewModel), _dbReader, Settings.Default.Language);
					throw;
				}
			}
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

		public List<Card> Cards
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

		public static Deck GetCurrentDeck()
		{
			return _deck;
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
			_processCount++;
			var document = new FlowDocument();

			foreach (Card card in Deck.MainBoard)
			{
				var exCard = new ExCard(_compressor, _dbReader, card, _dbWriter, _imageParse);
				byte[] img = exCard.Image;
				if (img != null)
				{
					document.Blocks.Add(new BlockUIContainer(new Image { Source = img.ToBitmapImage(), Width = 312, Height = 445 }));
				}
			}
			foreach (Card card in Deck.SideBoard)
			{
				var exCard = new ExCard(_compressor, _dbReader, card, _dbWriter, _imageParse);
				byte[] img = exCard.Image;
				if (img != null)
				{
					document.Blocks.Add(new BlockUIContainer(new Image { Source = img.ToBitmapImage(), Width = 312, Height = 445 }));
				}
			}

			using (var fs = new FileStream(DateTime.Now.ToFileTime() + ".rtf", FileMode.Create))
			{
				var sw = new StreamWriter(fs);
				sw.Write(document.ToRTF());
				sw.Flush();
			}

			_processCount--;
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
			_processCount++;
			new Thread(() =>
			{
				IEnumerable<Card> result = _dbReader.LoadCards();

				IEnumerable<Set> sets = FilterViewModel.Instance.Sets.Where(s => s.IsChecked).Select(s => s.Content);
				IEnumerable<Color> colors = FilterViewModel.Instance.Colors.Where(s => s.IsChecked).Select(s => s.Content);
				IEnumerable<Type> types = FilterViewModel.Instance.Types.Where(s => s.IsChecked).Select(s => s.Content);
				IEnumerable<Rarity> rarities = FilterViewModel.Instance.Rarities.Where(s => s.IsChecked).Select(s => s.Content);

				result = result.Where(c => sets.Any(s => s.SetCode == c.SetCode));

				result = result.Where(c => rarities.Any(r => r == c.GetRarity()));

				result = FilterViewModel.Instance.IncludeUnselectedColors
					? result.Where(c => c.GetColors().Any(colors.Contains))
					: result.Where(c => c.GetColors().All(colors.Contains));

				result = FilterViewModel.Instance.IncludeUnselectedTypes
					? result.Where(c => c.GetTypes().Any(types.Contains))
					: result.Where(c => c.GetTypes().All(types.Contains));

				switch (FilterViewModel.Instance.CMCCondition)
				{
					case FilterViewModel.CMCCompareType.EqualTo:
						result = result.Where(c => string.CompareOrdinal(c.CMC, FilterViewModel.Instance.Cost.ToString()) == 0);
						break;
					case FilterViewModel.CMCCompareType.NoLessThan:
						result = result.Where(c => string.CompareOrdinal(c.CMC, FilterViewModel.Instance.Cost.ToString()) >= 0);
						break;
					default:
						result = result.Where(c => string.CompareOrdinal(c.CMC, FilterViewModel.Instance.Cost.ToString()) <= 0);
						break;
				}

				if (!string.IsNullOrWhiteSpace(Input))
				{
					if (Regex.IsMatch(Input, @"@"))
					{
						foreach (string exp in Regex.Split(Input, @"&"))
						{
							string[] cons = Regex.Split(exp, @"@");
							if (cons.Length == 2)
							{
								switch (cons[0].ToLower())
								{
									case "name":
										result = result.Where(c => Regex.IsMatch(c.Name, cons[1], RegexOptions.IgnoreCase));
										break;
									case "type":
										result = result.Where(c => Regex.IsMatch(c.Type, cons[1], RegexOptions.IgnoreCase));
										break;
									case "txt":
										result = result.Where(c => Regex.IsMatch(c.Text, cons[1], RegexOptions.IgnoreCase));
										break;
									case "id":
										result = result.Where(c => Regex.IsMatch(c.ID, cons[1], RegexOptions.IgnoreCase));
										break;
									case "cost":
										result = result.Where(c => Regex.IsMatch(c.Cost, cons[1], RegexOptions.IgnoreCase));
										break;
									case "set":
										result = result.Where(c => Regex.IsMatch(c.Set, cons[1], RegexOptions.IgnoreCase));
										break;
									case "setc":
										result = result.Where(c => Regex.IsMatch(c.SetCode, cons[1], RegexOptions.IgnoreCase));
										break;
									case "fla":
										result = result.Where(c => Regex.IsMatch(c.Flavor, cons[1], RegexOptions.IgnoreCase));
										break;
									case "cmc":
										result = result.Where(c => Regex.IsMatch(c.CMC, cons[1], RegexOptions.IgnoreCase));
										break;
									case "rar":
										result = result.Where(c => Regex.IsMatch(c.Rarity, cons[1], RegexOptions.IgnoreCase));
										break;
									case "col":
										result =
											result.Where(c => c.GetColors().Select(col => col.ToString()).Any(co => Regex.IsMatch(co, cons[1], RegexOptions.IgnoreCase)));
										break;
									case "tgh":
										result = result.Where(c => Regex.IsMatch(c.Tgh, cons[1], RegexOptions.IgnoreCase));
										break;
									case "pow":
										result = result.Where(c => Regex.IsMatch(c.Pow, cons[1], RegexOptions.IgnoreCase));
										break;
									case "loy":
										result = result.Where(c => Regex.IsMatch(c.Loyalty, cons[1], RegexOptions.IgnoreCase));
										break;
									case "num":
										result = result.Where(c => Regex.IsMatch(c.Number, cons[1], RegexOptions.IgnoreCase));
										break;
									case "art":
										result = result.Where(c => Regex.IsMatch(c.Artist, cons[1], RegexOptions.IgnoreCase));
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

				Cards = result.ToList();

				_processCount--;
			}).Start();
		}

		private void NewDeckExecute()
		{
			Deck = new Deck();
		}

		private void OpenDeckExecute()
		{
			_processCount++;
			var dlg = new OpenFileDialog();

			dlg.Filter =
				_deckReaders.Aggregate("",
					(current, deckWriter) =>
						current + string.Format("|{0}(*.{1})|*.{2}", deckWriter.DeckType, deckWriter.FileExt, deckWriter.FileExt))
					.Remove(0, 1);
			dlg.RestoreDirectory = true;

			if (dlg.ShowDialog() == true)
			{
				try
				{
					IDeckReader deckReader = _deckReaders[dlg.FilterIndex - 1];
					using (var fs = (FileStream)dlg.OpenFile())
					{
						Deck = deckReader.Read(fs, Cards);
					}
				}
				catch (Exception ex)
				{
					Logger.Log(ex, typeof(DeckBuiderViewModel), _deckReaders[dlg.FilterIndex - 1]);
					throw;
				}
			}
			_processCount--;
		}

		private void SaveDeckExecute()
		{
			_processCount++;
			var dlg = new SaveFileDialog();

			dlg.Filter =
				_deckWriters.Aggregate("",
					(current, deckWriter) =>
						current + string.Format("|{0}(*.{1})|*.{2}", deckWriter.DeckType, deckWriter.FileExt, deckWriter.FileExt))
					.Remove(0, 1);
			dlg.RestoreDirectory = true;

			if (dlg.ShowDialog() == true)
			{
				try
				{
					IDeckWriter deckWriter = _deckWriters[dlg.FilterIndex - 1];
					using (var fs = (FileStream)dlg.OpenFile())
					{
						deckWriter.Write(Deck, fs);
					}
				}
				catch (Exception ex)
				{
					Logger.Log(ex, typeof(DeckBuiderViewModel), _deckWriters[dlg.FilterIndex - 1]);
					throw;
				}

				Info = "File successfully saved";
			}
			_processCount--;
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
			return _processCount == 0 && Deck.MainBoard.Count + Deck.SideBoard.Count > 0 && _compressor != null && _dbReader != null &&
				   _dbWriter != null && _imageParse != null;
		}

		private bool CanExecuteCopyImage()
		{
			return _processCount == 0 && SelectedCard.Image != null;
		}

		private bool CanExecuteClearInput()
		{
			return _processCount == 0 && !string.IsNullOrWhiteSpace(Input);
		}

		private bool CanExecuteFilter()
		{
			return _processCount == 0 && _dbReader != null;
		}

		private bool CanExecuteNewDeck()
		{
			return _processCount == 0 && Deck.MainBoard.Count > 0 || Deck.SideBoard.Count > 0;
		}

		private bool CanExecuteOpenDeck()
		{
			return _processCount == 0 && _deckReaders != null && _deckReaders.Length > 0 && Cards.Count > 0;
		}

		private bool CanExecuteSaveDeck()
		{
			return _processCount == 0 && _deckWriters != null && _deckWriters.Length > 0 && (Deck.MainBoard.Count > 0 || Deck.SideBoard.Count > 0);
		}

		private bool CanExecuteDeleteCardMain(Card card)
		{
			return _processCount == 0 && Deck.MainBoard.Contains(card);
		}

		private bool CanExecuteDeleteCardSide(Card card)
		{
			return _processCount == 0 && Deck.SideBoard.Contains(card);
		}

		private bool CanExecuteAddCardMain(Card card)
		{
			return _processCount == 0 && card != null;
		}

		private bool CanExecuteAddCardSide(Card card)
		{
			return _processCount == 0 && card != null;
		}

		#endregion
	}
}