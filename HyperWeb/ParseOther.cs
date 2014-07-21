using System;
using System.Collections.Generic;
using System.Linq;
using HyperKore.Common;
using HyperKore.Net;

namespace HyperKore.Web
{
	internal class ParseOther
	{
		public IEnumerable<string> ParseFormat()
		{
			return Parse("Card Format:").ToArray();
		}

		public IEnumerable<string> ParseSet()
		{
			return Parse("Card Set:").ToArray();
		}

		public IEnumerable<Set> ParseSetWithCode()
		{
			string webdata = Request.Instance.GetWebData(BuildURL(WEBSITE.magiccards));

			if (webdata.Contains("<label for=\"edition\">Edition:</label>"))
			{
				webdata = webdata.Substring(webdata.IndexOf("<label for=\"edition\">Edition:</label>"),
					webdata.IndexOf("<i>Use SHIFT and CTRL to select more than one edition.</i>") -
					webdata.IndexOf("<label for=\"edition\">Edition:</label>"));
				webdata = webdata.Substring(webdata.IndexOf("<optgroup"));

				while (webdata.Contains("value="))
				{
					string set = string.Empty;
					int num = webdata.IndexOf("/en\">") + 5;
					int num2 = webdata.IndexOf("</option>", num);
					int num3 = webdata.IndexOf("<option value=") + 16;
					int num4 = webdata.IndexOf("/en\">", num3);
					set = string.Format("{0}({1})", webdata.Substring(num, num2 - num), webdata.Substring(num3, num4 - num3).ToUpper());
					webdata = webdata.Substring(num2);
					if (!string.IsNullOrWhiteSpace(set))
					{
						string[] result = set.Split(new[] {'(', ')'}, StringSplitOptions.RemoveEmptyEntries);
						if (result.Length == 2)
						{
							yield return new Set {SetName = result[0].Trim(), SetCode = result[1].Trim()};
						}
					}
				}
			}
		}

		public IEnumerable<string> ParseType()
		{
			return Parse("Card Type:").ToArray();
		}

		private string BuildURL(WEBSITE website)
		{
			switch (website)
			{
				case WEBSITE.gatherer:
					return @"http://gatherer.wizards.com/Pages/Default.aspx";
				case WEBSITE.magiccards:
					return @"http://magiccards.info/search.html";
				default:
					throw new ArgumentOutOfRangeException("website");
			}
		}

		private IEnumerable<string> Parse(string header)
		{
			string webdata = Request.Instance.GetWebData(BuildURL(WEBSITE.gatherer));

			if (!webdata.Contains(header))
			{
				yield break;
			}

			int startidx = webdata.IndexOf(header);
			int endidx = webdata.IndexOf("</select>", startidx);
			webdata = webdata.Substring(webdata.IndexOf("<select", startidx), endidx - startidx);

			int idxa = webdata.IndexOf("<option");
			while (idxa > 0)
			{
				idxa = webdata.IndexOf(">", idxa) + 1;
				int idxb = webdata.IndexOf("</option>", idxa);
				string content = webdata.Substring(idxa, idxb - idxa);
				if (!string.IsNullOrWhiteSpace(content))
				{
					yield return content.Trim();
				}

				idxa = webdata.IndexOf("<option", idxb);
			}
		}
	}
}