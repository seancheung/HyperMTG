using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using HyperKore.Common;

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
		public static IEnumerable<COLOR> GetColors(this Card card)
		{
			if (card == null || card.Cost == null)
				throw new ArgumentNullException();

			if (!Regex.Match(card.Cost, @"W|B|U|R|G").Success)
			{
				yield return COLOR.Colorless;
				yield break;
			}
			if (card.Cost.Contains("W"))
				yield return COLOR.White;
			if (card.Cost.Contains("B"))
				yield return COLOR.Black;
			if (card.Cost.Contains("U"))
				yield return COLOR.Blue;
			if (card.Cost.Contains("R"))
				yield return COLOR.Red;
			if (card.Cost.Contains("G"))
				yield return COLOR.Green;
		}

		/// <summary>
		/// Get card types.
		/// Including super-types.
		/// </summary>
		/// <param name="card"></param>
		/// <returns></returns>
		public static IEnumerable<TYPE> GetTypes(this Card card)
		{
			if (card == null || card.Type == null)
				throw new ArgumentNullException();

			return from type in Enum.GetNames(typeof (TYPE)) where Regex.Match(card.Type, string.Format(@"\s{0}\s", type), RegexOptions.IgnoreCase).Success select (TYPE) Enum.Parse(typeof (TYPE), type);
		}

		/// <summary>
		/// Get rarity of a card. 
		/// Basic Land count as Common;
		/// Special count as Mythic Rare.
		/// </summary>
		/// <param name="card"></param>
		/// <returns></returns>
		public static RARITY GetRarity(this Card card)
		{
			if (card == null || card.Type == null)
				throw new ArgumentNullException();

			switch (card.Rarity.ToLower())
			{
				case "common":
				case "basic":
				case "land":
				case "basic land":
					return RARITY.Common;
				case "uncommon":
					return RARITY.Uncommon;
				case "rare":
					return RARITY.Rare;
				case "mythic rare":
				case "special":
					return RARITY.Mythic;
				default:
					throw new ArgumentException(card.Rarity);
			}
		}

		/// <summary>
		/// Whether the card is multicolored
		/// </summary>
		/// <param name="card"></param>
		/// <returns></returns>
		public static bool IsMultiColored(this Card card)
		{
			return card.GetColors().Count() > 1;
		}
	}
}