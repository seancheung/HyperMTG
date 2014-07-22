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
		public static string GetLangCode(this LANGUAGE lang)
		{
			switch (lang)
			{
				case LANGUAGE.ChineseSimplified:
					return "cn";

				case LANGUAGE.ChineseTraditional:
					return "tw";

				case LANGUAGE.German:
					return "ge";

				case LANGUAGE.French:
					return "fr";

				case LANGUAGE.Italian:
					return "it";

				case LANGUAGE.Japanese:
					return "jp";

				case LANGUAGE.Korean:
					return "ko";

				case LANGUAGE.Portuguese:
					return "pt";

				case LANGUAGE.Russian:
					return "ru";

				case LANGUAGE.Spanish:
					return "sp";

				default:
					return "en";
			}
		}

		public static string GetLangName(this LANGUAGE lang)
		{
			switch (lang)
			{
				case LANGUAGE.ChineseSimplified:
					return "Chinese Simplified";
				case LANGUAGE.ChineseTraditional:
					return "Chinese Traditional";
				default:
					return lang.ToString();
			}
		}
	}
}