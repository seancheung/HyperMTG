using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using HyperKore.Common;
using Type = HyperKore.Common.Type;

namespace HyperKore.Utilities
{
	public static class CardTool
	{
		/// <summary>
		///     Copy a card's properties' values from target card
		/// </summary>
		/// <param name="card"></param>
		/// <param name="target"></param>
		public static void CopyFrom(this Card card, Card target)
		{
			if (card == null || target == null || ReferenceEquals(card, target))
				return;

			foreach (PropertyInfo p in typeof(Card).GetProperties())
			{
				//Make sure it's readable
				if (p.CanWrite)
					p.SetValue(card, typeof(Card).GetProperty(p.Name).GetValue(target, null), null);
			}
		}

		/// <summary>
		/// Get card colors
		/// </summary>
		/// <param name="card"></param>
		/// <returns></returns>
		public static IEnumerable<Color> GetColors(this Card card)
		{
			if (card == null)
				throw new ArgumentNullException();

			if (card.Cost == null)
			{
				yield return Color.Colorless;
				yield break;
			}

			if (card.Cost.Contains("W"))
				yield return Color.White;
			if (card.Cost.Contains("B"))
				yield return Color.Black;
			if (card.Cost.Contains("U"))
				yield return Color.Blue;
			if (card.Cost.Contains("R"))
				yield return Color.Red;
			if (card.Cost.Contains("G"))
				yield return Color.Green;
		}

		/// <summary>
		/// Check card color
		/// </summary>
		/// <param name="card"></param>
		/// <param name="color"></param>
		/// <returns></returns>
		public static bool HasColor(this Card card, Color color)
		{
			return card.GetColors().Contains(color);
		}

		/// <summary>
		/// Check card type
		/// </summary>
		/// <param name="card"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool HasType(this Card card, Type type)
		{
			return card.GetTypes().Contains(type);
		}

		/// <summary>
		/// Get card types.
		/// Including super-types.
		/// </summary>
		/// <param name="card"></param>
		/// <returns></returns>
		public static IEnumerable<Type> GetTypes(this Card card)
		{
			if (card == null || card.Type == null)
				throw new ArgumentNullException();

			return from type in Enum.GetNames(typeof (Type)) where Regex.Match(card.Type, string.Format(@"\s*{0}\s*", type), RegexOptions.IgnoreCase).Success select (Type) Enum.Parse(typeof (Type), type);
		}

		/// <summary>
		/// Whether this card is a basic land
		/// </summary>
		/// <param name="card"></param>
		/// <returns></returns>
		public static bool IsBasicLand(this Card card)
		{
			if (card == null || card.Type == null)
				throw new ArgumentNullException();

			return card.GetTypes().Contains(Type.Basic) && card.GetTypes().Contains(Type.Land);
		}

		/// <summary>
		/// Get rarity of a card. 
		/// Basic Land count as Common;
		/// Special count as Mythic Rare.
		/// </summary>
		/// <param name="card"></param>
		/// <returns></returns>
		public static Rarity GetRarity(this Card card)
		{
			if (card == null || card.Type == null)
				throw new ArgumentNullException();

			switch (card.Rarity.ToLower())
			{
				case "common":
				case "basic":
				case "land":
				case "basic land":
					return Rarity.Common;
				case "uncommon":
					return Rarity.Uncommon;
				case "rare":
					return Rarity.Rare;
				case "mythic rare":
				case "special":
					return Rarity.Mythic;
				default:
					return Rarity.Common;
			}
		}

		/// <summary>
		/// Whether this card is multicolored
		/// </summary>
		/// <param name="card"></param>
		/// <returns></returns>
		public static bool IsMultiColored(this Card card)
		{
			return card.GetColors().Count() > 1;
		}

		/// <summary>
		/// Whether this card is hybrid
		/// </summary>
		/// <param name="card"></param>
		/// <returns></returns>
		public static bool IsHybrid(this Card card)
		{
			return card != null && Regex.IsMatch(card.Cost, @"{\D+}", RegexOptions.IgnoreCase);
		}

		/// <summary>
		/// Whether this card produce any mana
		/// </summary>
		/// <param name="card"></param>
		/// <returns></returns>
		public static bool CanProduceMana(this Card card)
		{
			return card != null && (card.IsBasicLand() || !string.IsNullOrWhiteSpace(card.Text) && Regex.IsMatch(card.Text, @"add .+ to .* mana pool", RegexOptions.IgnoreCase));
		}
	}
}