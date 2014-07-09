using HyperKore.Common;
using HyperKore.IO;
using HyperKore.Utilities;
using HyperKore.Xception;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MagicOnline
{
	public class MO : IDeckReader, IDeckWriter
	{
		public string DeckType
		{
			get { return "Magic Online"; }
		}

		public string Description
		{
			get { return "Import/Export MO format deck"; }
		}

		public string FileExt
		{
			get { return "txt"; }
		}

		public string Name
		{
			get { return "MO"; }
		}

		public Deck Read(Stream input, IEnumerable<Card> database)
		{
			Deck deck = new Deck();
			try
			{
				var odeck = Open(input);
				if (odeck.MainBoard.Count + odeck.SideBoard.Count > 0)
				{
					foreach (var item in odeck.MainBoard)
					{
						for (int i = 0; i < item.Count; i++)
						{
							deck.MainBoard.Add(Convert(item, database));
						}
					}
					foreach (var item in odeck.SideBoard)
					{
						for (int i = 0; i < item.Count; i++)
						{
							deck.SideBoard.Add(Convert(item, database));
						}
					}
					deck.Name = odeck.Name;
				}
				return deck;
			}
			catch
			{
				throw;
			}
		}

		public bool Write(Deck deck, Stream output)
		{
			try
			{
				var odeck = Convert(deck);
				Export(odeck, output);
				return true;
			}
			catch
			{
				return false;
			}
		}

		private Card Convert(MOCard card, IEnumerable<Card> database)
		{
			var res = database.FirstOrDefault(c => card.Name == c.GetLegalName());
			if (res != null)
			{
				return res;
			}
			else
			{
				throw new CardMissingXception("Card not found when loading MO deck.", card.Name);
			}
		}

		private MODeck Convert(Deck deck)
		{
			try
			{
				MODeck odeck = new MODeck();
				var lpM = deck.MainBoard.ToLookup(c => c.ID);
				foreach (var gp in lpM)
				{
					var tcard = gp.First();
					MOCard ocard = new MOCard() { Name = tcard.Name, Count = gp.Count() };
					odeck.MainBoard.Add(ocard);
				}

				var lpS = deck.SideBoard.ToLookup(c => c.ID);
				foreach (var gp in lpS)
				{
					var tcard = gp.First();
					MOCard ocard = new MOCard() { Name = tcard.Name, Count = gp.Count() };
					odeck.SideBoard.Add(ocard);
				}

				odeck.Name = deck.Name;

				return odeck;
			}
			catch
			{
				throw;
			}
		}

		private void Export(MODeck deck, Stream stream)
		{
			try
			{
				var sw = new StreamWriter(stream);

				deck.MainBoard.ForEach(c => sw.WriteLine(String.Format("{0} {1}", c.Count, c.Name)));

				sw.WriteLine("Sideboard");

				deck.SideBoard.ForEach(c => sw.WriteLine(String.Format("{0} {1}", c.Count, c.Name)));

				sw.Flush();
			}
			catch (Exception ex)
			{
				throw new IOXception("IO Error happended when exporting MO file", ex);
			}
		}

		private MODeck Open(Stream input)
		{
			try
			{
				var sr = new StreamReader(input);
				sr.BaseStream.Seek(0L, SeekOrigin.Begin);
				MODeck deck = new MODeck();

				var line = sr.ReadLine();
				int partID = 0; // 0 - main, 1 - side
				while (line != null)
				{
					if (line.Contains("Sideboard"))
					{
						partID = 1;
					}
					else if (!string.IsNullOrWhiteSpace(line))
					{
						MOCard card = new MOCard();

						if (partID == 0)
						{
							var idx = line.IndexOf(" ");
							if (idx > 0)
							{
								var count = line.Remove(idx).Trim();
								var name = line.Substring(idx).Trim();
								int cnt = 0;
								if (Int32.TryParse(count, out cnt))
								{
									card.Count = cnt;
									card.Name = name;

									deck.MainBoard.Add(card);
								}
							}
						}
						else
						{
							var idx = line.IndexOf(" ");
							if (idx > 0)
							{
								var count = line.Remove(idx).Trim();
								var name = line.Substring(idx).Trim();
								int cnt;
								if (Int32.TryParse(count, out cnt))
								{
									card.Count = cnt;
									card.Name = name;

									deck.SideBoard.Add(card);
								}
							}
						}
					}

					line = sr.ReadLine();
				}

				return deck;
			}
			catch (Exception ex)
			{
				throw new IOXception("IO Error happended when opening MO file", ex);
			}
		}
	}

	internal class MOCard
	{
		public int Count
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}
	}

	internal class MODeck
	{
		/// <summary>
		/// Initializes a new instance of the MODeck class.
		/// </summary>
		public MODeck(IEnumerable<MOCard> mainBoard, IEnumerable<MOCard> sideBoard, string name = "")
		{
			Name = name;
			MainBoard = new List<MOCard>(mainBoard);
			SideBoard = new List<MOCard>(sideBoard);
		}

		/// <summary>
		/// Initializes a new instance of the MODeck class.
		/// </summary>
		public MODeck()
		{
			Name = String.Empty;
			MainBoard = new List<MOCard>();
			SideBoard = new List<MOCard>();
		}

		public List<MOCard> MainBoard
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public List<MOCard> SideBoard
		{
			get;
			set;
		}
	}
}