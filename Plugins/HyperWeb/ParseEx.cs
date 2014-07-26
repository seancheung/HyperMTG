using System;
using System.Linq;
using System.Reflection;
using HyperKore.Common;
using HyperKore.Utilities;

namespace HyperPlugin.Web
{
	internal class ParseEx : ICardParse
	{
		#region ICardParse Members

		public void Parse(Card card, LANGUAGE lang)
		{
			ParseMana(card);
			ParseColor(card);
			ParseType(card);
			ParseRarity(card);
			ParseCharacters(card);
			RemoveEmptyProp(card);
		}

		#endregion

		private void ParseCharacters(Card card)
		{
			card.Name = card.Name.ReplaceSpecial();
			card.Text = card.Text.ReplaceSpecial();
			card.Flavor = card.Flavor.ReplaceSpecial();
			card.Rulings = card.Rulings.ReplaceSpecial();
		}

		private void ParseColor(Card card)
		{
			if (string.IsNullOrWhiteSpace(card.Color))
			{
				card.Color = "Colorless ";
				card.ColorCode = "C";
			}
			if (card.Cost.Contains("W"))
			{
				//card.Color += "White ";
				card.ColorCode += "W";
			}
			if (card.Cost.Contains("U"))
			{
				//card.Color += "Blue ";
				card.ColorCode += "U";
			}
			if (card.Cost.Contains("B"))
			{
				//card.Color += "Black ";
				card.ColorCode += "B";
			}
			if (card.Cost.Contains("R"))
			{
				//card.Color += "Red ";
				card.ColorCode += "R";
			}
			if (card.Cost.Contains("G"))
			{
				//card.Color += "Green ";
				card.ColorCode += "G";
			}

			card.Color = card.Color.Trim();
			if (card.IsDoubleFaced())
			{
				card.Color = String.Format("{0}|{1}", card.Color, card.ColorBside);
			}
			card.Color = card.Color.Trim();
		}

		private void ParseMana(Card card)
		{
			card.Cost = card.Cost
				.ReplaceColor()
				.Replace("{", string.Empty)
				.Replace("}", string.Empty)
				.Replace(" or ", string.Empty)
				.Replace("/", string.Empty);

			bool flag = false;
			string cost = string.Empty;
			string entry = string.Empty;

			foreach (char c in card.Cost)
			{
				if (c == '(')
				{
					flag = true;
					entry = string.Empty;
					continue;
				}
				if (c == ')')
				{
					flag = false;
					cost += string.Concat("{", entry, "}");
					continue;
				}

				if (flag)
				{
					entry += c;
				}
				else
				{
					if (c == '|' || c == '/')
					{
						cost += c;
					}
					else
					{
						cost += string.Concat("{", c, "}");
					}
				}
			}

			card.Cost = cost;

			if (card.zText.Contains('{'))
			{
				card.zText = card.zText.ReplaceMana();
			}
			if (card.Text.Contains('{'))
			{
				card.Text = card.Text.ReplaceMana();
			}
		}

		private void ParseRarity(Card card)
		{
			if (card.Rarity.Contains("Common"))
			{
				card.RarityCode = "C";
			}
			else if (card.Rarity.Contains("Uncommon"))
			{
				card.RarityCode = "U";
			}
			else if (card.Rarity.Contains("Mythic"))
			{
				card.RarityCode = "M";
			}
			else if (card.Rarity.Contains("Rare"))
			{
				card.RarityCode = "R";
			}
			else if (card.Rarity.Contains("Special"))
			{
				card.RarityCode = "S";
			}
			else if (card.Rarity.Contains("Basic"))
			{
				card.RarityCode = "B";
			}
		}

		private void ParseType(Card card)
		{
			if (card.Type.Contains("Legendary"))
			{
				card.TypeCode += "U";
			}

			if (card.Type.Contains("Tribal"))
			{
				card.TypeCode += "T";
			}

			if (card.Type.Contains("Snow"))
			{
				card.TypeCode += "O";
			}

			if (card.Type.Contains("World"))
			{
				card.TypeCode += "W";
			}

			if (card.Type.Contains("Plane") && !card.Type.Contains("Planeswalker"))
			{
				card.TypeCode += "N";
				return;
			}

			if (card.Type.Contains("Planeswalker"))
			{
				card.TypeCode += "P";
				return;
			}

			if (card.Type.Contains("Artifact"))
			{
				card.TypeCode += "A";
			}

			if (card.Type.Contains("Equipment"))
			{
				card.TypeCode += "Q";
			}

			if (card.Type.Contains("Enchantment"))
			{
				card.TypeCode += "E";
			}

			if (card.Type.Contains("Aura"))
			{
				card.TypeCode += "R";
			}

			if (card.Type.Contains("Land"))
			{
				if (card.Type.Contains("Basic"))
				{
					if (card.Type.Contains("Plains"))
					{
						card.TypeCode += "BL1";
					}
					if (card.Type.Contains("Island"))
					{
						card.TypeCode += "BL2";
					}
					if (card.Type.Contains("Swamp"))
					{
						card.TypeCode += "BL3";
					}
					if (card.Type.Contains("Mountain"))
					{
						card.TypeCode += "BL4";
					}
					if (card.Type.Contains("Forest"))
					{
						card.TypeCode += "BL5";
					}
				}
				else
				{
					card.TypeCode += "L";
				}

				card.CMC = "0";
			}

			if (card.Type.Contains("Creature"))
			{
				card.TypeCode += "C";
			}

			if (card.Type.Contains("Instant"))
			{
				card.TypeCode += "I";
			}

			if (card.Type.Contains("Sorcery"))
			{
				card.TypeCode += "S";
			}
		}

		private void RemoveEmptyProp(Card card)
		{
			foreach (PropertyInfo prop in typeof (Card).GetProperties())
			{
				if (prop.CanWrite)
				{
					if (prop.GetValue(card, null).ToString() == string.Empty)
					{
						prop.SetValue(card, null, null);
					}
				}
			}
		}
	}
}