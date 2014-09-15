using HyperKore.Common;

namespace HyperKore.Utilities
{
	public static class LanguageTool
	{
		/// <summary>
		///     Get language code
		/// </summary>
		/// <param name="lang"></param>
		/// <returns></returns>
		public static string GetLangCode(this Language lang)
		{
			switch (lang)
			{
				case Language.ChineseSimplified:
					return "cn";

				case Language.ChineseTraditional:
					return "tw";

				case Language.German:
					return "ge";

				case Language.French:
					return "fr";

				case Language.Italian:
					return "it";

				case Language.Japanese:
					return "jp";

				case Language.Korean:
					return "ko";

				case Language.Portuguese:
					return "pt";

				case Language.Russian:
					return "ru";

				case Language.Spanish:
					return "sp";

				default:
					return "en";
			}
		}

		/// <summary>
		/// Get language full name
		/// </summary>
		/// <param name="lang"></param>
		/// <returns></returns>
		public static string GetLangName(this Language lang)
		{
			switch (lang)
			{
				case Language.ChineseSimplified:
					return "Chinese Simplified";
				case Language.ChineseTraditional:
					return "Chinese Traditional";
				default:
					return lang.ToString();
			}
		}

		/// <summary>
		/// Get language form language code
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public static Language GetLangugeByCode(string code)
		{
			switch (code)
			{
				case "cn":
					return Language.ChineseSimplified;

				case "tw":
					return Language.ChineseTraditional;

				case "ge":
					return Language.German;

				case "fr":
					return Language.French;

				case "it":
					return Language.Italian;

				case "jp":
					return Language.Japanese;

				case "ko":
					return Language.Korean;

				case "pt":
					return Language.Portuguese;

				case "ru":
					return Language.Russian;

				case "sp":
					return Language.Spanish;

				default:
					return Language.English;
			}
		}
	}
}