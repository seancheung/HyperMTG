using HyperKore.Net;
using System.Collections.Generic;
using System.Linq;

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

		public IEnumerable<string> ParseType()
		{
			return Parse("Card Type:").ToArray();
		}

		private string BuildURL()
		{
			return @"http://gatherer.wizards.com/Pages/Default.aspx";
		}

		private IEnumerable<string> Parse(string header)
		{
			string webdata;
			try
			{
				webdata = Request.Instance.GetWebData(BuildURL());
			}
			catch
			{
				throw;
			}

			if (!webdata.Contains(header))
			{
				yield break;
			}

			var startidx = webdata.IndexOf(header);
			var endidx = webdata.IndexOf("</select>", startidx);
			webdata = webdata.Substring(webdata.IndexOf("<select", startidx), endidx - startidx);

			var idxa = webdata.IndexOf("<option");
			while (idxa > 0)
			{
				idxa = webdata.IndexOf(">", idxa) + 1;
				var idxb = webdata.IndexOf("</option>", idxa);
				var content = webdata.Substring(idxa, idxb - idxa);
				if (!string.IsNullOrWhiteSpace(content))
				{
					yield return content.Trim();
				}

				idxa = webdata.IndexOf("<option", idxb);
			}
		}
	}
}