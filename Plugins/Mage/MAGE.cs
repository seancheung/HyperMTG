using HyperKore.Common;
using HyperKore.IO;
using HyperKore.Utilities;
using HyperKore.Xception;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mage
{
	public sealed class MAGE : IDeckReader, IDeckWriter
	{
		public string DeckType
		{
			get { return "Mage"; }
		}

		public string Description
		{
			get { return "Import/Export Mage format deck"; }
		}

		public string FileExt
		{
			get { return "txt"; }
		}

		public string Name
		{
			get { return "MAGE"; }
		}

		public Deck Read(Stream input, IEnumerable<Card> database)
		{
			Deck deck = new Deck();
			try
			{
				var gdeck = Open(input);
				if (gdeck.MainBoard.Count + gdeck.SideBoard.Count > 0)
				{
					foreach (var item in gdeck.MainBoard)
					{
						for (int i = 0; i < item.Count; i++)
						{
							deck.MainBoard.Add(Convert(item, database));
						}
					}
					foreach (var item in gdeck.SideBoard)
					{
						for (int i = 0; i < item.Count; i++)
						{
							deck.SideBoard.Add(Convert(item, database));
						}
					}
					deck.Name = gdeck.Name;
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
				var gdeck = Convert(deck);
				Export(gdeck, output);
				return true;
			}
			catch
			{
				return false;
			}
		}

		private Card Convert(MAGECard card, IEnumerable<Card> database)
		{
			var res = database.FirstOrDefault(c => card.SetCode == c.SetCode && card.Name == c.GetLegalName());
			if (res != null)
			{
				return res;
			}
			else
			{
				throw new CardMissingXception("Card not found when loading Mage deck.", card.Name, card.SetCode);
			}
		}

		private MAGEDeck Convert(Deck deck)
		{
			try
			{
				MAGEDeck gdeck = new MAGEDeck();
				var lpM = deck.MainBoard.ToLookup(c => c.ID);
				foreach (var gp in lpM)
				{
					var tcard = gp.First();
					MAGECard gcard = new MAGECard() { Name = tcard.Name, SetCode = tcard.SetCode, Number = tcard.Number, Count = gp.Count() };
					gdeck.MainBoard.Add(gcard);
				}

				var lpS = deck.SideBoard.ToLookup(c => c.ID);
				foreach (var gp in lpS)
				{
					var tcard = gp.First();
					MAGECard gcard = new MAGECard() { Name = tcard.Name, SetCode = tcard.SetCode, Number = tcard.Number, Count = gp.Count() };
					gdeck.SideBoard.Add(gcard);
				}

				gdeck.Name = deck.Name;

				return gdeck;
			}
			catch
			{
				throw;
			}
		}

		private void Export(MAGEDeck deck, Stream stream)
		{
			try
			{
				var sw = new StreamWriter(stream);

				sw.WriteLine("NAME: " + deck.Name);

				deck.MainBoard.ForEach(c => sw.WriteLine(String.Format("{0} [{1}:{2}] {3}", c.Count, c.SetCode, c.Number, c.Name)));

				deck.SideBoard.ForEach(c => sw.WriteLine(String.Format("SB: {0} [{1}:{2}] {3}", c.Count, c.SetCode, c.Number, c.Name)));

				sw.Flush();
			}
			catch (Exception ex)
			{
				throw new IOXception("IO Error happended when exporting MAGE file", ex);
			}
		}

		private MAGEDeck Open(Stream input)
		{
			try
			{
				var sr = new StreamReader(input);
				sr.BaseStream.Seek(0L, SeekOrigin.Begin);
				MAGEDeck deck = new MAGEDeck();

				var line = sr.ReadLine();

				while (line != null)
				{
					if (line.Contains("["))
					{
						MAGECard card = new MAGECard();

						if (line.Contains("SB:"))
						{
							var idxa = line.IndexOf("[");
							var idxb = line.IndexOf("]");

							if (idxa > 0 && idxb > idxa)
							{
								var idxc = line.IndexOf(":", idxa);
								var idxd = line.IndexOf(":");
								if (idxc > idxa && idxc < idxb)
								{
									var count = line.Substring(idxd + 1, idxa - idxd - 1).Trim();
									var setcode = line.Substring(idxa + 1, idxc - idxa - 1).Trim();
									var number = line.Substring(idxc + 1, idxb - idxc - 1).Trim();
									var name = line.Substring(idxb + 1).Replace("SB:", string.Empty).Trim();
									int cnt;

									if (Int32.TryParse(count, out cnt))
									{
										card.Name = name;
										card.Count = cnt;
										card.Number = number;
										card.SetCode = setcode;

										deck.SideBoard.Add(card);
									}
								}
							}
						}
						else
						{
							var idxa = line.IndexOf("[");
							var idxb = line.IndexOf("]");

							if (idxa > 0 && idxb > idxa)
							{
								var idxc = line.IndexOf(":", idxa);
								if (idxc > idxa && idxc < idxb)
								{
									var count = line.Remove(idxa).Trim();
									var setcode = line.Substring(idxa + 1, idxc - idxa - 1).Trim();
									var number = line.Substring(idxc + 1, idxb - idxc - 1).Trim();
									var name = line.Substring(idxb + 1).Trim();
									int cnt;

									if (Int32.TryParse(count, out cnt))
									{
										card.Name = name;
										card.Count = cnt;
										card.Number = number;
										card.SetCode = setcode;

										deck.MainBoard.Add(card);
									}
								}
							}
						}
					}
					else if (!string.IsNullOrWhiteSpace(line))
					{
						deck.Name = line.Replace("NAME:", string.Empty).Replace("name:", string.Empty).Trim();
					}

					line = sr.ReadLine();
				}

				return deck;
			}
			catch (Exception ex)
			{
				throw new IOXception("IO Error happended when opening mage file", ex);
			}
		}
	}

	internal class MAGECard
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

		public string Number
		{
			get;
			set;
		}

		public string SetCode
		{
			get;
			set;
		}
	}

	internal class MAGEDeck
	{
		/// <summary>
		/// Initializes a new instance of the MageDeck class.
		/// </summary>
		public MAGEDeck(IEnumerable<MAGECard> mainBoard, IEnumerable<MAGECard> sideBoard, string name = "")
		{
			Name = name;
			MainBoard = new List<MAGECard>(mainBoard);
			SideBoard = new List<MAGECard>(sideBoard);
		}

		/// <summary>
		/// Initializes a new instance of the MageDeck class.
		/// </summary>
		public MAGEDeck()
		{
			Name = String.Empty;
			MainBoard = new List<MAGECard>();
			SideBoard = new List<MAGECard>();
		}

		public List<MAGECard> MainBoard
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public List<MAGECard> SideBoard
		{
			get;
			set;
		}
	}
}