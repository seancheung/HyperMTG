using System.Text.RegularExpressions;

namespace HyperMTGMain.Helper
{
	public class BBCodeHelper
	{
		public static string ToXaml(string content)
		{
			string result = Regex.Replace(content, @"<", "&lt;", RegexOptions.IgnoreCase);
			result = Regex.Replace(result, @">", "&gt;", RegexOptions.IgnoreCase);
			result = Regex.Replace(result, @"&", "&amp;", RegexOptions.IgnoreCase);
			result = Regex.Replace(result, "\"", "&quot;", RegexOptions.IgnoreCase);

			result = Regex.Replace(result, @"\[b\]", "<Bold>", RegexOptions.IgnoreCase);
			result = Regex.Replace(result, @"\[/b\]", "</Bold>", RegexOptions.IgnoreCase);
			result = Regex.Replace(result, @"\[i\]", "<Italic>", RegexOptions.IgnoreCase);
			result = Regex.Replace(result, @"\[/i\]", "</Italic>", RegexOptions.IgnoreCase);
			result = Regex.Replace(result, @"\[u\]", "<Underline>", RegexOptions.IgnoreCase);
			result = Regex.Replace(result, @"\[/u\]", "</Underline>", RegexOptions.IgnoreCase);
			result = Regex.Replace(result, @"(\r\n)+|\r+|\n+", "<LineBreak/>", RegexOptions.IgnoreCase | RegexOptions.Multiline);

			return string.Format("<Paragraph>{0}</Paragraph>", result);
		}
	}
}