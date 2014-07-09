namespace HyperKore.Utilities
{
	public static class StringTool
	{
		/// <summary>
		/// Replace color expressions like 'White' with colorcodes like 'W'
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
		/// Replace colorcodes like 'W' with expressions like 'White'
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
		/// Replace cost expressions like '{Blue}' with symbols like '{U}'
		/// </summary>
		/// <param name="cost"></param>
		/// <returns></returns>
		public static string ReplaceCost(this string cost)
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
		/// Replace cost symbols like '{U}' with expressions like '{Blue}'
		/// </summary>
		/// <param name="symbol"></param>
		/// <returns></returns>
		public static string ReplaceCostBack(this string symbol)
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
		/// Replace special characters like 'a' with legal ones
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string ReplaceSpecial(this string text)
		{
			return text
				   .Replace("?", "o")
				   .Replace("a", "a")
				   .Replace("á", "a")
				   .Replace("í", "i")
				   .Replace("ú", "u")
				   .Replace("?", "u")
				   .Replace("?", "AE")
				   .Replace("é", "e")
				   .Replace("à", "a");
		}
	}
}