using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HyperKore.Common;
using HyperKore.Exception;
using IOException = HyperKore.Exception.IOException;

namespace HyperPlugin
{
	public class MO : IDeckReader, IDeckWriter
	{
		#region IDeckReader Members

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
			var deck = new Deck();
			MODeck odeck = Open(input);
			if (odeck.MainBoard.Count + odeck.SideBoard.Count > 0)
			{
				foreach (MOCard item in odeck.MainBoard)
				{
					for (int i = 0; i < item.Count; i++)
					{
						deck.MainBoard.Add(Convert(item, database));
					}
				}
				foreach (MOCard item in odeck.SideBoard)
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

		#endregion

		#region IDeckWriter Members

		public bool Write(Deck deck, Stream output)
		{
			try
			{
				MODeck odeck = Convert(deck);
				Export(odeck, output);
				return true;
			}
			catch
			{
				return false;
			}
		}

		#endregion

		private Card Convert(MOCard card, IEnumerable<Card> database)
		{
			Card res = database.FirstOrDefault(c => card.Name == c.Name);
			if (res != null)
			{
				return res;
			}
			throw new CardMissingException();
		}

		private MODeck Convert(Deck deck)
		{
			var odeck = new MODeck();
			ILookup<string, Card> lpM = deck.MainBoard.ToLookup(c => c.ID);
			foreach (var gp in lpM)
			{
				Card tcard = gp.First();
				var ocard = new MOCard {Name = tcard.Name, Count = gp.Count()};
				odeck.MainBoard.Add(ocard);
			}

			ILookup<string, Card> lpS = deck.SideBoard.ToLookup(c => c.ID);
			foreach (var gp in lpS)
			{
				Card tcard = gp.First();
				var ocard = new MOCard {Name = tcard.Name, Count = gp.Count()};
				odeck.SideBoard.Add(ocard);
			}

			odeck.Name = deck.Name;

			return odeck;
		}

		private void Export(MODeck deck, Stream stream)
		{
			try
			{
				var sw = new StreamWriter(stream);

				deck.MainBoard.ForEach(c => sw.WriteLine("{0} {1}", c.Count, c.Name));

				sw.WriteLine("Sideboard");

				deck.SideBoard.ForEach(c => sw.WriteLine("{0} {1}", c.Count, c.Name));

				sw.Flush();
			}
			catch (Exception ex)
			{
				throw new IOException(ex);
			}
		}

		private MODeck Open(Stream input)
		{
			try
			{
				var sr = new StreamReader(input);
				sr.BaseStream.Seek(0L, SeekOrigin.Begin);
				var deck = new MODeck();

				string line = sr.ReadLine();
				int partID = 0; // 0 - main, 1 - side
				while (line != null)
				{
					if (line.Contains("Sideboard"))
					{
						partID = 1;
					}
					else if (!string.IsNullOrWhiteSpace(line))
					{
						var card = new MOCard();

						if (partID == 0)
						{
							int idx = line.IndexOf(" ");
							if (idx > 0)
							{
								string count = line.Remove(idx).Trim();
								string name = line.Substring(idx).Trim();
								int cnt;
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
							int idx = line.IndexOf(" ");
							if (idx > 0)
							{
								string count = line.Remove(idx).Trim();
								string name = line.Substring(idx).Trim();
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
				throw new IOException(ex);
			}
		}
	}

	internal class MOCard
	{
		public int Count { get; set; }

		public string Name { get; set; }
	}

	internal class MODeck
	{
		/// <summary>
		///     Initializes a new instance of the MODeck class.
		/// </summary>
		public MODeck(IEnumerable<MOCard> mainBoard, IEnumerable<MOCard> sideBoard, string name = "")
		{
			Name = name;
			MainBoard = new List<MOCard>(mainBoard);
			SideBoard = new List<MOCard>(sideBoard);
		}

		/// <summary>
		///     Initializes a new instance of the MODeck class.
		/// </summary>
		public MODeck()
		{
			Name = String.Empty;
			MainBoard = new List<MOCard>();
			SideBoard = new List<MOCard>();
		}

		public List<MOCard> MainBoard { get; set; }

		public string Name { get; set; }

		public List<MOCard> SideBoard { get; set; }
	}
}