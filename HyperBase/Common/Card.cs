using System.Data.Linq.Mapping;
using System.Xml.Serialization;

namespace HyperKore.Common
{
	/// <summary>
	/// Card class that contains all basic info
	/// </summary>
	[Table(Name = "Card")]
	public class Card
	{
		[XmlIgnore]
		public string ColorBside;

		/// <summary>
		/// Artist name of the card
		/// </summary>
		[Column(Name = "artist")]
		[XmlAttribute("artist")]
		public string Artist
		{
			get;
			set;
		}

		/// <summary>
		/// Converted mana cost of the card (use use '|' as separator for dual, e.g. '3|2')
		/// </summary>
		[Column(Name = "cmc")]
		[XmlAttribute("cmc")]
		public string CMC
		{
			get;
			set;
		}

		/// <summary>
		/// Full english color name of the card (use use ' ' as separator for multi-color, e.g.
		/// 'Blue Red') (use use '|' as separator for dual, e.g. 'Blue|Black')
		/// </summary>
		[Column(Name = "color")]
		[XmlAttribute("color")]
		public string Color
		{
			get;
			set;
		}

		/// <summary>
		/// Colorcode in capital (no separator needed for multi-color, e.g. 'UR') (use use '|' as
		/// separator for dual, e.g. 'U|B')
		/// </summary>
		[Column(Name = "colorcode")]
		[XmlAttribute("colorcode")]
		public string ColorCode
		{
			get;
			set;
		}

		/// <summary>
		/// Cost of the card (use '{}' for each mana symbol, e.g. '{3}{B}{R}') (bracket hybrid mana
		/// symbol as one, e.g. '{WU}') (use use '|' as separator for dual, e.g. '{1}{W}|{2}{G}{G}')
		/// </summary>
		[Column(Name = "cost")]
		[XmlAttribute("cost")]
		public string Cost
		{
			get;
			set;
		}

		/// <summary>
		/// English flavor of the card
		/// </summary>
		[Column(Name = "flavor")]
		[XmlAttribute("flavor")]
		public string Flavor
		{
			get;
			set;
		}

		/// <summary>
		/// English WotcID of the card (use '|' as separator for dual, e.g. '12345|67890')
		/// </summary>
		[Column(Name = "id", IsPrimaryKey = true)]
		[XmlAttribute("id")]
		public string ID
		{
			get;
			set;
		}

		/// <summary>
		/// Legality of the card
		/// </summary>
		[Column(Name = "legality")]
		[XmlAttribute("legality")]
		public string Legality
		{
			get;
			set;
		}

		/// <summary>
		/// Loyalty of the card(planeswalker) (use use '|' as separator for dual, e.g. '3|0')
		/// </summary>
		[Column(Name = "loyalty")]
		[XmlAttribute("loyalty")]
		public string Loyalty
		{
			get;
			set;
		}

		/// <summary>
		/// English name of the card (use '|' as separator for dual, e.g. 'ABC|DEF')
		/// </summary>
		[Column(Name = "name")]
		[XmlAttribute("name")]
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Number of the card (use use '|' as separator for dual, e.g. '121a|121b')
		/// </summary>
		[Column(Name = "number")]
		[XmlAttribute("number")]
		public string Number
		{
			get;
			set;
		}

		/// <summary>
		/// Power of the card(creature) (use use '|' as separator for dual, e.g. '1|3')
		/// </summary>
		[Column(Name = "pow")]
		[XmlAttribute("pow")]
		public string Pow
		{
			get;
			set;
		}

		/// <summary>
		/// Rarity of the card
		/// </summary>
		[Column(Name = "rarity")]
		[XmlAttribute("rarity")]
		public string Rarity
		{
			get;
			set;
		}

		/// <summary>
		/// Raritycode of the card
		/// </summary>
		[Column(Name = "raritycode")]
		[XmlAttribute("raritycode")]
		public string RarityCode
		{
			get;
			set;
		}

		/// <summary>
		/// Community rating of the card
		/// </summary>
		[Column(Name = "rating")]
		[XmlAttribute("rating")]
		public string Rating
		{
			get;
			set;
		}

		/// <summary>
		/// Rulings of the card
		/// </summary>
		[Column(Name = "rulings")]
		[XmlAttribute("rulings")]
		public string Rulings
		{
			get;
			set;
		}

		/// <summary>
		/// Full english set name of the card (use '|' as separator for dual, e.g. 'ABC|DEF')
		/// </summary>
		[Column(Name = "set")]
		[XmlAttribute("set")]
		public string Set
		{
			get;
			set;
		}

		/// <summary>
		/// Setcode in capital
		/// </summary>
		[Column(Name = "setcode")]
		[XmlAttribute("setcode")]
		public string SetCode
		{
			get;
			set;
		}

		/// <summary>
		/// English text of the card
		/// </summary>
		[Column(Name = "text")]
		[XmlAttribute("text")]
		public string Text
		{
			get;
			set;
		}

		/// <summary>
		/// Toughness of the card(creature) (use use '|' as separator for dual, e.g. '1|3')
		/// </summary>
		[Column(Name = "tgh")]
		[XmlAttribute("tgh")]
		public string Tgh
		{
			get;
			set;
		}

		/// <summary>
		/// Type of the card (use use '|' as separator for dual, e.g. 'Creature — Human
		/// Advisor|Creature — Human Mutant')
		/// </summary>
		[Column(Name = "type")]
		[XmlAttribute("type")]
		public string Type
		{
			get;
			set;
		}

		/// <summary>
		/// Typecode in capital (no separator needed for multi-type, e.g. 'AC') (use use '|' as
		/// separator for dual, e.g. 'C|C')
		/// </summary>
		[Column(Name = "typecode")]
		[XmlAttribute("typecode")]
		public string TypeCode
		{
			get;
			set;
		}

		/// <summary>
		/// Variation of the card(for basic land card) (in the format of '(1:373546)(2:373609)(3:373683)(4:373746)')
		/// </summary>
		[Column(Name = "var")]
		[XmlAttribute("var")]
		public string Var
		{
			get;
			set;
		}

		/// <summary>
		/// Foreign flavor of the card
		/// </summary>
		[Column(Name = "zflavor")]
		[XmlAttribute("zflavor")]
		public string zFlavor
		{
			get;
			set;
		}

		/// <summary>
		/// Foreign WotcID of the card (use '|' as separator for dual, e.g. '12345|67890')
		/// </summary>
		[Column(Name = "zid")]
		[XmlAttribute("zid")]
		public string zID
		{
			get;
			set;
		}
		/// <summary>
		/// Foreign name of the card (use '|' as separator for dual, e.g. 'ABC|DEF')
		/// </summary>
		[Column(Name = "zname")]
		[XmlAttribute("zname")]
		public string zName
		{
			get;
			set;
		}
		/// <summary>
		/// Foreign text of the card
		/// </summary>
		[Column(Name = "ztext")]
		[XmlAttribute("ztext")]
		public string zText
		{
			get;
			set;
		}

		/// <summary>
		/// Type of the card in foreign (use use '|' as separator for dual, e.g. 'Creature — Human
		/// Advisor|Creature — Human Mutant')
		/// </summary>
		[Column(Name = "ztype")]
		[XmlAttribute("ztype")]
		public string zType
		{
			get;
			set;
		}
	}
}