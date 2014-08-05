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
		public static string ToShortColor(this string color)
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
		public static string ToLongColor(this string symbol)
		{
			return symbol
				.Replace("C", "Colorless")
				.Replace("W", "White")
				.Replace("G", "Green")
				.Replace("B", "Black")
				.Replace("U", "Blue")
				.Replace("R", "Red")
				.Trim();
		}

		/// <summary>
		///     Replace special characters like 'Æ' with legal ones
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string ReplaceSpecialCharacter(this string text)
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

				sb.Append("{" + cost.ToShortColor() + "}");
			}

			return sb.ToString();
		}

		/// <summary>
		///     Split input string by mana symbols
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static IEnumerable<string> ManaSplit(this string input)
		{
			//return Regex.Split(input, "(?<=})|(?={)").Where(s => s != "");
			return Regex.Split(input, @"(?<=})|(?!^)(?={)");
		}

		/// <summary>
		///     Format mana cost string into a standard type
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string ManaFormat(this string input)
		{
			//e.g. {Black or White} => {B or W} => {BW}
			string text = Regex.Replace(input, @"(?<={)Green|Green(?=})", "G", RegexOptions.IgnoreCase);
			text = Regex.Replace(text, @"(?<={)Black|Black(?=})", "B", RegexOptions.IgnoreCase);
			text = Regex.Replace(text, @"(?<={)Blue|Blue(?=})", "U", RegexOptions.IgnoreCase);
			text = Regex.Replace(text, @"(?<={)Red|Red(?=})", "R", RegexOptions.IgnoreCase);
			text = Regex.Replace(text, @"(?<={)White|White(?=})", "W", RegexOptions.IgnoreCase);
			text = Regex.Replace(text, @"(?<={)Tap|Tap(?=})", "T", RegexOptions.IgnoreCase);
			text = Regex.Replace(text, @"(?<={[^}]*)\s+or\s+|/|\|(?=[^{]*})", "", RegexOptions.IgnoreCase);

			return text;
		}

		/// <summary>
		/// Build Mana strings
		/// e.g. 1(Black)(White) => {1}{Black}{White}
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string ManaBuild(this string input)
		{
			string text = input;
			if (!Regex.IsMatch(text, @"{|}") || Regex.IsMatch(text, @"\(\)"))
			{
				foreach (Match match in Regex.Matches(text, @"\d+"))
				{
					text = Regex.Replace(text, match.Value, "{" + match.Value + "}");
				}

			}

			text = Regex.Replace(text, @"\(", "{");
			text = Regex.Replace(text, @"\)", "}");
			text = Regex.Replace(text, @"{{2,}", "{");
			text = Regex.Replace(text, @"}{2,}", "}");

			return text;
		}
	}
}