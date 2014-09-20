using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using HyperKore.Common;
using HyperKore.Logger;
using HyperMTGMain.Helper;
using HyperMTGMain.Model;
using HyperPlugin;
using Microsoft.Win32;

namespace HyperMTGMain.ViewModel
{
	public class DeckEditorViewModel : ObservableClass
	{
		private static DeckEditorViewModel _instance;
		private List<Card> _cards;
		private Deck _deck;
		private string _input;
		private bool _isFoil;
		private ImgCard _selectedCard;

		private DeckEditorViewModel()
		{
			Deck = new Deck();
			SelectedCard = new ImgCard();
		}

		internal static DeckEditorViewModel Instance
		{
			get { return _instance ?? (_instance = new DeckEditorViewModel()); }
		}

		#region Notifiable

		public ImgCard SelectedCard
		{
			get { return _selectedCard; }
			set
			{
				_selectedCard = value;
				OnPropertyChanged("SelectedCard");
			}
		}

		public Deck Deck
		{
			get { return _deck; }
			set
			{
				_deck = value;
				OnPropertyChanged("Deck");
			}
		}

		public List<Card> Cards
		{
			get { return _cards; }
			set
			{
				_cards = value;
				OnPropertyChanged("Cards");
			}
		}

		public string Input
		{
			get { return _input; }
			set
			{
				_input = value;
				OnPropertyChanged("Input");
			}
		}

		public bool IsFoil
		{
			get { return _isFoil; }
			set
			{
				_isFoil = value;
				OnPropertyChanged("IsFoil");
			}
		}

		#endregion

		#region Commands

		public ICommand OpenCommand
		{
			get { return new RelayCommand(Open, CanOpen); }
		}

		public ICommand SaveCommand
		{
			get { return new RelayCommand(Save, CanSave); }
		}

		public ICommand NewCommand
		{
			get { return new RelayCommand(New, CanNew); }
		}

		public ICommand ClipboardImportCommand
		{
			get { return new RelayCommand(ClipboardImport, CanClipboardImport); }
		}

		public ICommand ClipboardExportCommand
		{
			get { return new RelayCommand(ClipboardExport, CanClipboardExport); }
		}

		public ICommand CopyImageCommand
		{
			get { return new RelayCommand(CopyImage, CanCopyImage); }
		}

		public ICommand MoveToSideCommand
		{
			get { return new RelayCommand<Card>(MoveToSide, CanDeleteMain); }
		}

		public ICommand MoveToMainCommand
		{
			get { return new RelayCommand<Card>(MoveToMain, CanDeleteSide); }
		}

		public ICommand AddMainCommand
		{
			get { return new RelayCommand<Card>(AddMain, CanAddMain); }
		}

		public ICommand AddSideCommand
		{
			get { return new RelayCommand<Card>(AddSide, CanAddSide); }
		}

		public ICommand DeleteMainCommand
		{
			get { return new RelayCommand<Card>(DeleteMain, CanDeleteMain); }
		}

		public ICommand DeleteSideCommand
		{
			get { return new RelayCommand<Card>(DeleteSide, CanDeleteSide); }
		}

		private bool CanOpen()
		{
			return TaskManager.Count <= 0 && PluginFactory.ComponentsAvailable;
		}

		private void Open()
		{
			var dlg = new OpenFileDialog();

			dlg.Filter =
				PluginFactory.DeckReaders.Aggregate("",
					(current, deckWriter) =>
						current + string.Format("|{0}(*.{1})|*.{2}", deckWriter.DeckType, deckWriter.FileExt, deckWriter.FileExt))
					.Remove(0, 1);
			dlg.RestoreDirectory = true;

			if (dlg.ShowDialog() == true)
			{
				try
				{
					IDeckReader deckReader = PluginFactory.DeckReaders.ToArray()[dlg.FilterIndex - 1];
					using (var fs = (FileStream) dlg.OpenFile())
					{
						Deck = deckReader.Read(fs, Cards);
					}
				}
				catch (Exception ex)
				{
					Logger.Log(ex, this, PluginFactory.DeckReaders.ToArray()[dlg.FilterIndex - 1]);
					throw;
				}
			}
		}

		private bool CanSave()
		{
			return TaskManager.Count <= 0 && PluginFactory.ComponentsAvailable &&
			       (Deck.MainBoard.Count > 0 || Deck.SideBoard.Count > 0);
		}

		private void Save()
		{
			var dlg = new SaveFileDialog();

			dlg.Filter =
				PluginFactory.DeckWriters.Aggregate("",
					(current, deckWriter) =>
						current + string.Format("|{0}(*.{1})|*.{2}", deckWriter.DeckType, deckWriter.FileExt, deckWriter.FileExt))
					.Remove(0, 1);
			dlg.RestoreDirectory = true;

			if (dlg.ShowDialog() == true)
			{
				try
				{
					IDeckWriter deckWriter = PluginFactory.DeckWriters.ToArray()[dlg.FilterIndex - 1];
					using (var fs = (FileStream) dlg.OpenFile())
					{
						deckWriter.Write(Deck, fs);
					}
				}
				catch (Exception ex)
				{
					Logger.Log(ex, this, PluginFactory.DeckWriters.ToArray()[dlg.FilterIndex - 1]);
					throw;
				}

				ViewModelManager.MessageViewModel.Inform("File successfully saved");
			}
		}

		private bool CanNew()
		{
			return TaskManager.Count <= 0 && Deck.MainBoard.Count > 0 || Deck.SideBoard.Count > 0;
		}

		private void New()
		{
			Deck = new Deck();
		}

		private bool CanClipboardImport()
		{
			return TaskManager.Count <= 0 && PluginFactory.ComponentsAvailable;
		}

		private void ClipboardImport()
		{
			string data = Clipboard.GetText(TextDataFormat.Text);
			if (!string.IsNullOrWhiteSpace(data))
			{
				IEnumerable<Card> db = PluginFactory.DbReader.LoadCards();
				Match side = Regex.Match(data, "sideboard:", RegexOptions.IgnoreCase);
				var mainCards = new List<Card>();
				var sideCards = new List<Card>();

				foreach (Match match in Regex.Matches(data, @"\d+\s+[^\d\r\n]+"))
				{
					if (match.Success)
					{
						int count = Int32.Parse(Regex.Match(match.Value, @"\d+(?=\s+[^\d\r\n]+)").Value);
						string name = Regex.Match(match.Value, @"(?<=\d+\s+)[^\d\r\n]+").Value.Trim();
						Card card = db.FirstOrDefault(c => c.Name == name);
						if (card != null)
						{
							if (side.Success && match.Index > side.Index)
							{
								for (int i = 0; i < count; i++)
								{
									sideCards.Add(card);
								}
							}
							else
							{
								for (int i = 0; i < count; i++)
								{
									mainCards.Add(card);
								}
							}
						}
						else
						{
							ViewModelManager.MessageViewModel.Message("{0} not found", name);
						}
					}
				}

				if (mainCards.Any() || sideCards.Any())
				{
					New();
					foreach (Card card in mainCards)
					{
						Deck.MainBoard.Add(card);
					}
					foreach (Card card in sideCards)
					{
						Deck.SideBoard.Add(card);
					}
				}
			}
		}

		private bool CanClipboardExport()
		{
			return Deck.SideBoard.Any() || Deck.SideBoard.Any();
		}

		private void ClipboardExport()
		{
			IEnumerable<IGrouping<string, Card>> main = Deck.MainBoard.GroupBy(c => c.Name);
			IEnumerable<IGrouping<string, Card>> side = Deck.SideBoard.GroupBy(c => c.Name);
			using (var sw = new StringWriter())
			{
				foreach (var gp in main)
				{
					sw.WriteLine("{0}\t{1}", gp.Count(), gp.Key);
				}
				if (side.Any())
				{
					sw.WriteLine("Sideboard:");
					foreach (var gp in side)
					{
						sw.WriteLine("{0}\t{1}", gp.Count(), gp.Key);
					}
				}

				Clipboard.SetText(sw.ToString());
			}
		}

		private bool CanCopyImage()
		{
			return TaskManager.Count <= 0 && SelectedCard.Image != null;
		}

		private void CopyImage()
		{
			Clipboard.SetImage(SelectedCard.Image.ToBitmapImage());
		}

		private void AddMain(Card card)
		{
			Deck.MainBoard.Add(card);
		}

		private void AddSide(Card card)
		{
			Deck.SideBoard.Add(card);
		}

		private void DeleteMain(Card card)
		{
			Deck.MainBoard.Remove(card);
		}

		private void DeleteSide(Card card)
		{
			Deck.SideBoard.Remove(card);
		}

		private void MoveToSide(Card card)
		{
			Deck.MainBoard.Remove(card);
			Deck.SideBoard.Add(card);
		}

		private void MoveToMain(Card card)
		{
			Deck.SideBoard.Remove(card);
			Deck.MainBoard.Add(card);
		}

		private bool CanDeleteMain(Card card)
		{
			return TaskManager.Count <= 0 && Deck.MainBoard.Contains(card);
		}

		private bool CanDeleteSide(Card card)
		{
			return TaskManager.Count <= 0 && Deck.SideBoard.Contains(card);
		}

		private bool CanAddMain(Card card)
		{
			return TaskManager.Count <= 0 && card != null;
		}

		private bool CanAddSide(Card card)
		{
			return TaskManager.Count <= 0 && card != null;
		}

		#endregion

		public void LoadCards()
		{
			if (!PluginFactory.ComponentsAvailable)
			{
				return;
			}
			try
			{
				Cards = PluginFactory.DbReader.LoadCards().ToList();
			}
			catch (Exception ex)
			{
				Logger.Log(ex, this);
				throw;
			}
		}
	}
}