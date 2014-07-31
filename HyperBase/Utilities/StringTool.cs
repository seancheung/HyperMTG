using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HyperKore.Utilities
{
	public static class StringTool
	{
		/// <summary>
		///     Replace color expressions like 'White' with colorcodes like 'W'
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static string ReplaceColor(this string color)
		{
			return color
				.Replace("White", "W")
				.Replace("Green", "G")
				.Replace("Blue", "U")
				.Replace("Red", "R")
				.Replace("Black", "B")
				.Replace("Colorless", "C")
				.Trim();
		}

		/// <summary>
		///     Replace colorcodes like 'W' with expressions like 'White'
		/// </summary>
		/// <param name="symbol"></param>
		/// <returns></returns>
		public static string ReplaceColorBack(this string symbol)
		{
			return symbol
				.Replace("W", "White")
				.Replace("G", "Green")
				.Replace("U", "Blue")
				.Replace("R", "Red")
				.Replace("B", "Black")
				.Replace("C", "Colorless")
				.Trim();
		}

		/// <summary>
		///     Replace cost expressions like '{Blue}' with symbols like '{U}'
		/// </summary>
		/// <param name="cost"></param>
		/// <returns></returns>
		public static string ReplaceMana(this string cost)
		{
			return cost
				.Replace("{Tap}", "{T}")
				.Replace("{White}", "{W}")
				.Replace("{Blue}", "{U}")
				.Replace("{Black}", "{B}")
				.Replace("{Red}", "{R}")
				.Replace("{Green}", "{G}");
		}

		/// <summary>
		///     Replace cost symbols like '{U}' with expressions like '{Blue}'
		/// </summary>
		/// <param name="symbol"></param>
		/// <returns></returns>
		public static string ReplaceManaBack(this string symbol)
		{
			return symbol
				.Replace("{T}", "{Tap}")
				.Replace("{W}", "{White}")
				.Replace("{U}", "{Blue}")
				.Replace("{B}", "{Black}")
				.Replace("{R}", "{Red}")
				.Replace("{G}", "{Green}");
		}

		/// <summary>
		///     Replace special characters like 'Æ' with legal ones
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string ReplaceSpecial(this string text)
		{
			return text
				.Replace("ō", "o")
				.Replace("ó", "o")
				.Replace("ǒ", "o")
				.Replace("ò", "o")
				.Replace("ā", "a")
				.Replace("á", "a")
				.Replace("ǎ", "a")
				.Replace("à", "a")
				.Replace("ī", "i")
				.Replace("í", "i")
				.Replace("ǐ", "i")
				.Replace("ì", "i")
				.Replace("ū", "u")
				.Replace("ú", "u")
				.Replace("ǔ", "u")
				.Replace("ù", "u")
				.Replace("Æ", "AE")
				.Replace("æ", "ae")
				.Replace("é", "e")
				.Replace("é", "e")
				.Replace("ě", "e")
				.Replace("è", "e");
		}

		/// <summary>
		///     Remove all html tags
		/// </summary>
		/// <param name="input">String to format</param>
		/// <returns></returns>
		public static string RemoveHtmlTag(this string input)
		{
			//Replace </div> with new line
			string text = Regex.Replace(input, @"</div>", "\n", RegexOptions.IgnoreCase);

			//Remove <*>
			text = Regex.Replace(text, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);

			//Replace multiple empty lines with one
			return Regex.Replace(text, @"([\r\n])[\s]+", "\n", RegexOptions.IgnoreCase).Trim();
		}

		/// <summary>
		///     Get all multiverse id from the input string
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static IEnumerable<string> MatchMultiverseID(this string input)
		{
			MatchCollection matches = Regex.Matches(input, @"multiverseid=[0-9]+");
			return from Match match in matches where match.Success select match.Value.Remove(0, 13);
		}

		/// <summary>
		///     Get all cost symbols from the input string
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string MatchCost(this string input)
		{
			MatchCollection matches = Regex.Matches(input, @"alt=""\w+""");
			var sb = new StringBuilder();
			foreach (Match match in matches)
			{
				string cost = match.Value.Remove(0, 4).Replace("\"", string.Empty).Replace("or", string.Empty);
				cost = Regex.Replace(cost, @"\s*", string.Empty);

				sb.Append("{" + cost.ReplaceColor() + "}");
			}

			return sb.ToString();
		}

		/// <summary>
		/// Split input string by mana symbols
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static IEnumerable<string> ManaSplit(this string input)
		{
			//return Regex.Split(input, "(?<=})|(?={)").Where(s => s != "");
			return Regex.Split(input, "(?<=})|(?!^)(?={)");
		}
	}
}