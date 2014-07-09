using HyperKore.Common;

namespace HyperKore.Utilities
{
	public static class LanguageTool
	{
		/// <summary>
		/// Get language code
		/// </summary>
		/// <param name="lang"></param>
		/// <returns></returns>
		public static string GetLangCode(this LANGUAGE lang)
		{
			string result = "en";

			switch (lang)
			{
				case LANGUAGE.ChineseSimplified:
					result = "cn";
					break;

				case LANGUAGE.ChineseTraditional:
					result = "tw";
					break;

				case LANGUAGE.German:
					result = "ge";
					break;

				case LANGUAGE.French:
					result = "fr";
					break;

				case LANGUAGE.Italian:
					result = "it";
					break;

				case LANGUAGE.Japanese:
					result = "jp";
					break;

				case LANGUAGE.Korean:
					result = "ko";
					break;

				case LANGUAGE.Portuguese:
					result = "pt";
					break;

				case LANGUAGE.Russian:
					result = "ru";
					break;

				case LANGUAGE.Spanish:
					result = "sp";
					break;

				case LANGUAGE.English:
					break;

				default:
					break;
			}

			return result;
		}
	}
}