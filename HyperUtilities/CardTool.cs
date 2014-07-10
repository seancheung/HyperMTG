using HyperKore.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HyperKore.Utilities
{
	public static class CardTool
	{
		/// <summary>
		/// Copy a card's properties' values from target card
		/// </summary>
		/// <param name="card"></param>
		/// <param name="target"></param>
		public static void CopyFrom(this Card card, Card target)
		{
			if (card == null || target == null || card == target)
			{
				return;
			}

			foreach (var p in typeof(Card).GetProperties())
			{
				//Make sure it's readable
				if (p.CanWrite)
					p.SetValue(card, typeof(Card).GetProperty(p.Name).GetValue(target, null), null);
			}
		}

		/// <summary>
		/// Get card name. If it's doublefaced, the first name will be returned
		/// </summary>
		/// <param name="card"></param>
		/// <returns></returns>
		public static string GetLegalName(this Card card)
		{
			if (card.IsDoubleFaced())
			{
				return card.GetNames().ToArray()[0];
			}
			else
			{
				return card.Name;
			}
		}

		/// <summary>
		/// Get split Names
		/// </summary>
		/// <param name="card"></param>
		/// <returns></returns>
		public static IEnumerable<string> GetNames(this Card card)
		{
			if (!card.IsDoubleFaced())
				yield return card.Name;
			else
			{
				foreach (var name in card.Name.Split('|'))
					yield return name;
			}
		}

		/// <summary>
		/// Whether this card is doublefaced
		/// </summary>
		/// <param name="card"></param>
		/// <returns></returns>
		public static bool IsDoubleFaced(this Card card)
		{
			return card.ID.Contains("|");
		}

		/// <summary>
		/// Whether this card is split
		/// </summary>
		/// <param name="card"></param>
		/// <returns></returns>
		public static bool IsSplit(this Card card)
		{
			return card.Cost.Contains("|") && !card.ID.Contains("|");
		}
	}
}