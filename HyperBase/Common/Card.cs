using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Xml.Serialization;

namespace HyperKore.Common
{
	/// <summary>
	///     Card class that contains all basic info
	/// </summary>
	[Table(Name = "Card")]
	public class Card
	{
		/// <summary>
		///     Raritycode of the card
		/// </summary>
		[Column(Name = "rarity")]
		[XmlAttribute("rarity")]
		public string Rarity { get; set; }

		/// <summary>
		///     Artist name of the card
		/// </summary>
		[Column(Name = "artist")]
		[XmlAttribute("artist")]
		public string Artist { get; set; }

		/// <summary>
		///     Converted mana cost of the card
		/// </summary>
		[Column(Name = "cmc")]
		[XmlAttribute("cmc")]
		public string CMC { get; set; }

		/// <summary>
		///     Cost of the card (use '{}' for each mana symbol, e.g. '{3}{B}{R}') (bracket hybrid mana
		///     symbol as one, e.g. '{WU}')
		/// </summary>
		[Column(Name = "cost")]
		[XmlAttribute("cost")]
		public string Cost { get; set; }

		/// <summary>
		///     English flavor of the card
		/// </summary>
		[Column(Name = "flavor")]
		[XmlAttribute("flavor")]
		public string Flavor { get; set; }

		/// <summary>
		///     English WotcID of the card
		/// </summary>
		[Column(Name = "id", IsPrimaryKey = true)]
		[XmlAttribute("id")]
		public string ID { get; set; }

		/// <summary>
		///     Loyalty of the card(planeswalker)
		/// </summary>
		[Column(Name = "loyalty")]
		[XmlAttribute("loyalty")]
		public string Loyalty { get; set; }

		/// <summary>
		///     English name of the card
		/// </summary>
		[Column(Name = "name")]
		[XmlAttribute("name")]
		public string Name { get; set; }

		/// <summary>
		///     Number of the card
		/// </summary>
		[Column(Name = "number")]
		[XmlAttribute("number")]
		public string Number { get; set; }

		/// <summary>
		///     Power of the card(creature)
		/// </summary>
		[Column(Name = "pow")]
		[XmlAttribute("pow")]
		public string Pow { get; set; }

		/// <summary>
		///     Full english set name of the card
		/// </summary>
		[Column(Name = "set")]
		[XmlAttribute("set")]
		public string Set { get; set; }

		/// <summary>
		///     Setcode in capital
		/// </summary>
		[Column(Name = "setcode")]
		[XmlAttribute("setcode")]
		public string SetCode { get; set; }

		/// <summary>
		///     English text of the card
		/// </summary>
		[Column(Name = "text")]
		[XmlAttribute("text")]
		public string Text { get; set; }

		/// <summary>
		///     Toughness of the card(creature)
		/// </summary>
		[Column(Name = "tgh")]
		[XmlAttribute("tgh")]
		public string Tgh { get; set; }

		/// <summary>
		///     Type of the card
		/// </summary>
		[Column(Name = "type")]
		[XmlAttribute("type")]
		public string Type { get; set; }

		#region Equality members

		protected bool Equals(Card other)
		{
			return string.Equals(ID, other.ID);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			var other = obj as Card;
			return other != null && Equals(other);
		}

		public override int GetHashCode()
		{
			return (ID != null ? ID.GetHashCode() : 0);
		}

		public static bool operator ==(Card left, Card right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Card left, Card right)
		{
			return !Equals(left, right);
		}

		#endregion

		#region IDEqualityComparer

		private static readonly IEqualityComparer<Card> IDComparerInstance = new IDEqualityComparer();

		public static IEqualityComparer<Card> IDComparer
		{
			get { return IDComparerInstance; }
		}

		private sealed class IDEqualityComparer : IEqualityComparer<Card>
		{
			public bool Equals(Card x, Card y)
			{
				if (ReferenceEquals(x, y)) return true;
				if (ReferenceEquals(x, null)) return false;
				if (ReferenceEquals(y, null)) return false;
				if (x.GetType() != y.GetType()) return false;
				return string.Equals(x.ID, y.ID);
			}

			public int GetHashCode(Card obj)
			{
				return (obj.ID != null ? obj.ID.GetHashCode() : 0);
			}
		}

		#endregion
	}
}