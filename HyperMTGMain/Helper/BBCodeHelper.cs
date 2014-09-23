using System.Text.RegularExpressions;

namespace HyperMTGMain.Helper
{
	public class BBCodeHelper
	{
		public static string ToXaml(string content)
		{
			string result = Normalize(content);
			result = CodeConvert(result);
			result = ImageConvert(result);

			return string.Format("<Paragraph>{0}</Paragraph>", result);
		}

		private static string Normalize(string content)
		{
			string result = Regex.Replace(content, @"<", "&lt;", RegexOptions.IgnoreCase);
			result = Regex.Replace(result, @">", "&gt;", RegexOptions.IgnoreCase);
			result = Regex.Replace(result, @"&", "&amp;", RegexOptions.IgnoreCase);
			result = Regex.Replace(result, "\"", "&quot;", RegexOptions.IgnoreCase);

			return result;
		}

		private static string CodeConvert(string content)
		{
			string result = Regex.Replace(content, @"\[b\]", "<Bold>");
			result = Regex.Replace(result, @"\[/b\]", "</Bold>");
			result = Regex.Replace(result, @"\[i\]", "<Italic>");
			result = Regex.Replace(result, @"\[/i\]", "</Italic>");
			result = Regex.Replace(result, @"\[u\]", "<Underline>");
			result = Regex.Replace(result, @"\[/u\]", "</Underline>");
			result = Regex.Replace(result, @"(\r\n)+|\r+|\n+", "<LineBreak/>", RegexOptions.IgnoreCase | RegexOptions.Multiline);

			return result;
		}

		private static string ImageConvert(string content)
		{
			string result = Regex.Replace(content, @"{B}",
				"<Path Style=\"{StaticResource PathB}\" Height=\"18\" Width=\"18\" Effect=\"{x:Null}\"/>");
			result = Regex.Replace(result, @"{G}",
				"<Path Style=\"{StaticResource PathG}\" Height=\"18\" Width=\"18\" Effect=\"{x:Null}\"/>");
			result = Regex.Replace(result, @"{R}",
				"<Path Style=\"{StaticResource PathR}\" Height=\"18\" Width=\"18\" Effect=\"{x:Null}\"/>");
			result = Regex.Replace(result, @"{U}",
				"<Path Style=\"{StaticResource PathU}\" Height=\"18\" Width=\"18\" Effect=\"{x:Null}\"/>");
			result = Regex.Replace(result, @"{W}",
				"<Path Style=\"{StaticResource PathW}\" Height=\"18\" Width=\"18\" Effect=\"{x:Null}\"/>");

			result = Regex.Replace(result, @"{BG}",
				"<Path Style=\"{StaticResource PathBG}\" Height=\"18\" Width=\"18\" Effect=\"{x:Null}\"/>");
			result = Regex.Replace(result, @"{BR}",
				"<Path Style=\"{StaticResource PathBR}\" Height=\"18\" Width=\"18\" Effect=\"{x:Null}\"/>");
			result = Regex.Replace(result, @"{GU}",
				"<Path Style=\"{StaticResource PathGU}\" Height=\"18\" Width=\"18\" Effect=\"{x:Null}\" />");
			result = Regex.Replace(result, @"{GW}",
				"<Path Style=\"{StaticResource PathGW}\" Height=\"18\" Width=\"18\" Effect=\"{x:Null}\"/>");
			result = Regex.Replace(result, @"{RG}",
				"<Path Style=\"{StaticResource PathRG}\" Height=\"18\" Width=\"18\" Effect=\"{x:Null}\"/>");
			result = Regex.Replace(result, @"{RW}",
				"<Path Style=\"{StaticResource PathRW}\" Height=\"18\" Width=\"18\" Effect=\"{x:Null}\"/>");
			result = Regex.Replace(result, @"{UB}",
				"<Path Style=\"{StaticResource PathUB}\" Height=\"18\" Width=\"18\" Effect=\"{x:Null}\"/>");
			result = Regex.Replace(result, @"{UR}",
				"<Path Style=\"{StaticResource PathUR}\" Height=\"18\" Width=\"18\" Effect=\"{x:Null}\"/>");
			result = Regex.Replace(result, @"{WB}",
				"<Path Style=\"{StaticResource PathWB}\" Height=\"18\" Width=\"18\" Effect=\"{x:Null}\"/>");
			result = Regex.Replace(result, @"{WU}",
				"<Path Style=\"{StaticResource PathWU}\" Height=\"18\" Width=\"18\" Effect=\"{x:Null}\"/>");

			return result;
		}
	}
}