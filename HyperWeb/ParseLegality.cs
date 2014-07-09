using HyperKore.Net;
using HyperKore.Xception;
using System;

namespace HyperKore.Web
{
	internal class ParseLegality : ICardParse
	{
		public void Parse(Common.Card card, Common.LANGUAGE lang)
		{
			string webdata;
			try
			{
				webdata = Request.Instance.GetWebData(BuildURL(card.ID));
			}
			catch
			{
				throw;
			}

			if (!webdata.Contains("This card has restrictions in the following formats"))
			{
				card.Legality = string.Empty;
				return;
			}

			try
			{
				webdata = webdata.Substring(webdata.IndexOf("This card has restrictions in the following formats"),
											webdata.IndexOf("For more information regarding each format and play style modifications")
											- webdata.IndexOf("This card has restrictions in the following formats"));
				while (webdata.Contains("<td style=\"text-align:center;\">"))
				{
					int num1 = webdata.IndexOf("<tr class=\"cardItem evenItem\">") + 30;
					num1 = webdata.IndexOf(">", num1) + 1;
					int num2 = webdata.IndexOf("</td>", num1);
					string arg = webdata.Substring(num1, num2 - num1).Trim();
					card.Legality += string.Format("[{0}]", arg);
					webdata = webdata.Substring(webdata.IndexOf("<td style=\"text-align:center;\">") + 30);
					if (webdata.Contains("<tr class=\"cardItem oddItem\">"))
					{
						int num3 = webdata.IndexOf("<tr class=\"cardItem oddItem\">") + 30;
						num3 = webdata.IndexOf(">", num3) + 1;
						int num4 = webdata.IndexOf("</td>", num3);
						arg = webdata.Substring(num3, num4 - num3).Trim();
						card.Legality += string.Format("[{0}]", arg);
						webdata = webdata.Substring(webdata.IndexOf("<td style=\"text-align:center;\">") + 30);
					}
				}
			}
			catch (Exception ex)
			{
				throw new ParsingXception("Parsing Error happended when parsing card Legality:", ex);
			}
		}

		private string BuildURL(string id)
		{
			if (id.Contains("|"))
			{
				id = id.Remove(id.IndexOf("|"));
			}

			return string.Format("http://gatherer.wizards.com/Pages/Card/Printings.aspx?multiverseid={0}", id);
		}
	}
}