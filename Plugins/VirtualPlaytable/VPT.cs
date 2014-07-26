using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using HyperKore.Common;
using HyperKore.Exception;
using HyperKore.Utilities;
using IOException = HyperKore.Exception.IOException;

namespace HyperPlugin.IO.VirtualPlaytable
{
	public class VPT : IDeckReader, IDeckWriter
	{
		#region IDeckReader Members

		public string DeckType
		{
			get { return "Virtual Playtable"; }
		}

		public string Description
		{
			get { return "Import/Export VPT format deck"; }
		}

		public string FileExt
		{
			get { return "deck"; }
		}

		public string Name
		{
			get { return "VPT"; }
		}

		public Deck Read(Stream input, IEnumerable<Card> database)
		{
			var deck = new Deck();
			try
			{
				VPTDeck vdeck = Open(input);
				foreach (VPTItem item in vdeck.Sections[0].Items)
				{
					for (int i = 0; i < item.Cards.Sum(c => c.Count); i++)
					{
						deck.MainBoard.Add(Convert(item, database));
					}
				}
				foreach (VPTItem item in vdeck.Sections[1].Items)
				{
					for (int i = 0; i < item.Cards.Sum(c => c.Count); i++)
					{
						deck.SideBoard.Add(Convert(item, database));
					}
				}

				deck.Name = vdeck.Name;
				MODE mode;
				if (Enum.TryParse(vdeck.Mode, true, out mode))
				{
					deck.Mode = mode;
				}
				FORMAT format;
				if (Enum.TryParse(vdeck.Format, true, out format))
				{
					deck.Format = format;
				}

				return deck;
			}
			catch
			{
				throw;
			}
		}

		#endregion

		#region IDeckWriter Members

		public bool Write(Deck deck, Stream output)
		{
			try
			{
				VPTDeck vdeck = Convert(deck);
				Export(vdeck, output);
				return true;
			}
			catch
			{
				return false;
			}
		}

		#endregion

		private Card Convert(VPTItem item, IEnumerable<Card> database)
		{
			Card res = database.FirstOrDefault(c => item.Cards.First().SetCode == c.SetCode && item.Name == c.GetLegalName());
			if (res != null)
			{
				return res;
			}
			throw new CardMissingException();
		}

		private VPTDeck Convert(Deck deck)
		{
			var sectionM = new VPTSection {ID = "main"};
			ILookup<string, Card> lpM = deck.MainBoard.ToLookup(c => c.ID);
			foreach (var gp in lpM)
			{
				Card tcard = gp.First();
				var vcard = new VPTCard(tcard.SetCode, LANGUAGE.English.GetLangCode(), tcard.Var, gp.Count());
				var vitem = new VPTItem {Name = tcard.Name};
				vitem.Cards.Add(vcard);
				sectionM.Items.Add(vitem);
			}

			var sectionS = new VPTSection {ID = "sideboard"};
			ILookup<string, Card> lpS = deck.SideBoard.ToLookup(c => c.ID);
			foreach (var gp in lpS)
			{
				Card tcard = gp.First();
				var vcard = new VPTCard(tcard.SetCode, LANGUAGE.English.GetLangCode(), tcard.Var, gp.Count());
				var vitem = new VPTItem {Name = tcard.Name};
				vitem.Cards.Add(vcard);
				sectionS.Items.Add(vitem);
			}

			var vdeck = new VPTDeck("mtg", deck.Mode.ToString(), deck.Format.ToString(), deck.Name, new List<VPTSection>
			{
				sectionM,
				sectionS
			});

			return vdeck;
		}

		private void Export(VPTDeck deck, Stream output)
		{
			try
			{
				var serializer = new XmlSerializer(typeof (VPTDeck));
				var nas = new XmlSerializerNamespaces();
				nas.Add(string.Empty, string.Empty);
				serializer.Serialize(output, deck, nas);
			}
			catch (Exception ex)
			{
				throw new IOException(ex);
			}
		}

		private VPTDeck Open(Stream input)
		{
			try
			{
				var serializer = new XmlSerializer(typeof (VPTDeck));
				var data = serializer.Deserialize(input) as VPTDeck;
				return data;
			}
			catch (Exception ex)
			{
				throw new IOException(ex);
			}
		}
	}

	[XmlType("card")]
	public class VPTCard
	{
		/// <summary>
		///     Initializes a new instance of the VPTCard class.
		/// </summary>
		public VPTCard(string setCode, string lang, string @var, int count)
		{
			SetCode = setCode;
			Lang = lang;
			Var = @var;
			Count = count;
		}

		/// <summary>
		///     Initializes a new instance of the VPTCard class.
		/// </summary>
		public VPTCard()
		{
			SetCode = String.Empty;
			Lang = String.Empty;
			Var = String.Empty;
			Count = 0;
		}

		[XmlAttribute("count")]
		public int Count { get; set; }

		[XmlAttribute("lang")]
		public string Lang { get; set; }

		[XmlAttribute("set")]
		public string SetCode { get; set; }

		[XmlAttribute("ver")]
		public string Var { get; set; }
	}

	[XmlRoot("deck")]
	public class VPTDeck
	{
		/// <summary>
		///     Initializes a new instance of the VPTDeck class.
		/// </summary>
		public VPTDeck(string game, string mode, string format, string name, List<VPTSection> sections)
		{
			Game = game;
			Mode = mode;
			Format = format;
			Name = name;
			Sections = sections;
		}

		/// <summary>
		///     Initializes a new instance of the VPTDeck class.
		/// </summary>
		public VPTDeck()
		{
			Game = String.Empty;
			Mode = String.Empty;
			Format = String.Empty;
			Name = String.Empty;
			Sections = new List<VPTSection>();
		}

		[XmlAttribute("format")]
		public string Format { get; set; }

		[XmlAttribute("game")]
		public string Game { get; set; }

		[XmlAttribute("mode")]
		public string Mode { get; set; }

		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlElement("section")]
		public List<VPTSection> Sections { get; set; }
	}

	[XmlType("item")]
	public class VPTItem
	{
		/// <summary>
		///     Initializes a new instance of the VPTItem class.
		/// </summary>
		public VPTItem(string name, List<VPTCard> cards)
		{
			Name = name;
			Cards = cards;
		}

		/// <summary>
		///     Initializes a new instance of the VPTItem class.
		/// </summary>
		public VPTItem()
		{
			Name = String.Empty;
			Cards = new List<VPTCard>();
		}

		[XmlElement("card")]
		public List<VPTCard> Cards { get; set; }

		[XmlAttribute("id")]
		public string Name { get; set; }
	}

	[XmlType("section")]
	public class VPTSection
	{
		/// <summary>
		///     Initializes a new instance of the VPTSection class.
		/// </summary>
		public VPTSection(string iD, List<VPTItem> items)
		{
			ID = iD;
			Items = items;
		}

		/// <summary>
		///     Initializes a new instance of the VPTSection class.
		/// </summary>
		public VPTSection()
		{
			ID = String.Empty;
			Items = new List<VPTItem>();
		}

		[XmlAttribute("id")]
		public string ID { get; set; }

		[XmlElement("item")]
		public List<VPTItem> Items { get; set; }
	}
}