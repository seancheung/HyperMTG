using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

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
			MainBoard = new ObservableCollection<Card>();
			SideBoard = new ObservableCollection<Card>();
			Comment = String.Empty;
			Format = Format.Default;
			Mode = Mode.Default;
		}

		[XmlAttribute("comment")]
		public string Comment
		{
			get;
			set;
		}

		[XmlAttribute("format")]
		public Format Format
		{
			get;
			set;
		}

		[XmlArray("mainboard"), XmlArrayItem("card")]
		public ICollection<Card> MainBoard { get; set; }

		[XmlAttribute("type")]
		public Mode Mode
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