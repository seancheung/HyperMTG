using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace HyperKore.Common
{
	[XmlRoot("hyperdeck")]
	public class Deck
	{
		/// <summary>
		/// Initializes a new instance of the Deck class.
		/// </summary>
		public Deck()
		{
			Name = String.Empty;
			MainBoard = new List<Card>();
			SideBoard = new List<Card>();
			Comment = String.Empty;
			Format = FORMAT.Default;
			Mode = MODE.Default;
		}

		[XmlAttribute("comment")]
		public string Comment
		{
			get;
			set;
		}

		[XmlAttribute("format")]
		public FORMAT Format
		{
			get;
			set;
		}

		[XmlArray("mainboard"), XmlArrayItem("card")]
		public ICollection<Card> MainBoard { get; set; }

		[XmlAttribute("type")]
		public MODE Mode
		{
			get;
			set;
		}

		[XmlAttribute("name")]
		public string Name
		{
			get;
			set;
		}
		[XmlArray("sideboard"), XmlArrayItem("card")]
		public ICollection<Card> SideBoard { get; set; }
	}
}