using System;
using HyperKore.Common;
using HyperKore.Exception;

namespace HyperPlugin.Web
{
	internal class ParseLegality : ICardParse
	{
		private readonly IRequest request;

		public ParseLegality()
		{
			request = PluginManager.Instance.GetPlugin<IRequest>();
			if (request == null)
			{
				throw new AssamlyMissingException();
			}
		}

		#region ICardParse Members

		public void Parse(ref Card card, LANGUAGE lang)
		{
			string webdata = request.GetWebData(BuildURL(card.ID));

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
				throw new ParsingException();
			}
		}

		#endregion

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