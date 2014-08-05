using System;
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
			card.Name = card.Name.ReplaceSpecialCharacter();
			card.Text = card.Text.ReplaceSpecialCharacter();
			card.Flavor = card.Flavor.ReplaceSpecialCharacter();
			card.Rulings = card.Rulings.ReplaceSpecialCharacter();
		}

		private void ParseColor(Card card)
		{
			if (string.IsNullOrWhiteSpace(card.Color))
			{
				card.Color = "Colorless ";
				card.ColorCode = "C";
			}

			card.ColorCode = card.Color.ToShortColor().Replace(" ", string.Empty);

			if (card.IsDoubleFaced())
			{
				card.Color = String.Format("{0}|{1}", card.Color, card.ColorBside);
			}
		}

		private void ParseMana(Card card)
		{
			card.Cost = card.Cost.ManaBuild();
			card.Cost = card.Cost.ManaFormat();
			card.Text = card.Text.ManaFormat();
			card.zText = card.zText.ManaFormat();
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
			//else if (card.Rarity.Contains("Special"))
			//{
			//	card.RarityCode = "S";
			//}
			//else if (card.Rarity.Contains("Basic"))
			//{
			//	card.RarityCode = "B";
			//}
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