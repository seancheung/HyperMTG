using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HyperKore.Common;

namespace HyperKore.Utilities
{
	public static class BoosterTool
	{
		#region BoosterType enum

		/// <summary>
		/// Type of the booster
		/// </summary>
		public enum BoosterType
		{
			/// <summary>
			/// Automatically resolve rules
			/// </summary>
			Default,

			/// <summary>
			/// Rare/Mythic Rare x 1; Uncommon x 3; Common x 11; Basic Land x 0;
			/// </summary>
			Mr1U3C11L0,

			/// <summary>
			/// Rare/Mythic Rare x 1; Uncommon x 3; Common x 10; Basic Land x 1;
			/// </summary>
			Mr1U3C10L1
		}

		#endregion

		#region ShiftDirection enum

		/// <summary>
		/// Direction to shift
		/// </summary>
		public enum ShiftDirection
		{
			Left,
			Right
		}

		#endregion

		/// <summary>
		/// Generate a booster pack
		/// </summary>
		/// <param name="database">Database to generate from</param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IEnumerable<Card> Generate(IEnumerable<Card> database, BoosterType type = BoosterType.Default)
		{
			if (database == null)
			{
				yield break;
			}

			Card[] mrCards = database.Where(c => c.GetRarity() == Rarity.Mythic || c.GetRarity() == Rarity.Rare).ToArray();
			Card[] uCards = database.Where(c => c.GetRarity() == Rarity.Uncommon).ToArray();
			Card[] cCards = database.Where(c => c.GetRarity() == Rarity.Common).ToArray();
			Card[] lCards = database.Where(c => c.IsBasicLand()).ToArray();

			int mrNum = 1;
			int uNum = 3;
			int cNum;
			int lNum;

			switch (type)
			{
				case BoosterType.Default:
					if (lCards.Any())
					{
						cNum = 10;
						lNum = 1;
					}
					else
					{
						cNum = 11;
						lNum = 0;
					}
					break;
				case BoosterType.Mr1U3C11L0:
					cNum = 11;
					lNum = 0;
					break;
				case BoosterType.Mr1U3C10L1:
					cNum = 10;
					lNum = 1;
					break;
				default:
					throw new ArgumentOutOfRangeException("type");
			}

			foreach (Card card in mrCards.GetRandoms(mrNum))
			{
				yield return card;
			}
			foreach (Card card in uCards.GetRandoms(uNum))
			{
				yield return card;
			}
			foreach (Card card in cCards.GetRandoms(cNum))
			{
				yield return card;
			}
			foreach (Card card in lCards.GetRandoms(lNum))
			{
				yield return card;
			}
		}

		/// <summary>
		/// Get Random cards from provided database
		/// </summary>
		/// <param name="cards">Database to generate from</param>
		/// <param name="count">Amount to generate</param>
		/// <param name="allowDuplicate">allow same card to appear </param>
		/// <returns></returns>
		public static IEnumerable<Card> GetRandoms(this IList<Card> cards, int count = 1, bool allowDuplicate = false)
		{
			if (cards == null)
			{
				throw new ArgumentNullException();
			}

			Random ran = new Random();

			for (int i = 0; i < count; i++)
			{
				int index = ran.Next(0, cards.Count);
				yield return cards[index];
				if (allowDuplicate)
				{
					cards.RemoveAt(index);
				}
			}
		}

		/// <summary>
		/// Shift list for specified times
		/// </summary>
		/// <param name="list"></param>
		/// <param name="direction"></param>
		/// <param name="k"></param>
		public static void ShiftList(IList list, ShiftDirection direction, int k = 1)
		{
			if (list == null)
			{
				return;
			}
			int size = list.Count - 1;

			switch (direction)
			{
				case ShiftDirection.Left:
					Reverse(list, 0, k);
					Reverse(list, k + 1, size);
					Reverse(list, 0, size);
					break;
				case ShiftDirection.Right:
					Reverse(list, 0, size - k);
					Reverse(list, size - k + 1, size);
					Reverse(list, 0, size);
					break;
				default:
					throw new ArgumentOutOfRangeException("direction");
			}
		}

		/// <summary>
		/// reverse a part of a list
		/// </summary>
		/// <param name="list"></param>
		/// <param name="left"></param>
		/// <param name="right"></param>
		public static void Reverse(IList list, int left, int right)
		{
			while (left <= right)
			{
				object l = list[left];
				object r = list[right];
				list.Insert(left, r);
				list.RemoveAt(left + 1);
				list.Insert(right, l);
				list.RemoveAt(right + 1);

				left++;
				right--;
			}
		}

		/// <summary>
		/// Get cards by ids
		/// </summary>
		/// <param name="db"></param>
		/// <param name="ids"></param>
		/// <returns></returns>
		public static IEnumerable<Card> GetCards(IEnumerable<Card> db, IEnumerable<string> ids)
		{
			if (db == null)
			{
				yield break;
			}

			foreach (string id in ids)
			{
				Card card = db.FirstOrDefault(c => c.ID == id);
				if (card != null)
				{
					yield return card;
				}
			}
		} 
	}
}