using HyperKore.Common;
using HyperKore.Net;
using HyperKore.Xception;
using System;

namespace HyperKore.Web
{
	internal class ParsezID : ICardParse
	{
		public void Parse(Card card, LANGUAGE lang)
		{
			GetzID(card, lang);

			//use traditional chinese in case of simplified being unavailable
			if (card.zID == string.Empty && lang == LANGUAGE.ChineseSimplified)
			{
				GetzID(card, LANGUAGE.ChineseTraditional);
			}
		}

		private string BuildURL(string id)
		{
			if (id.Contains("|"))
			{
				id = id.Remove(id.IndexOf("|"));
			}

			return string.Format("http://gatherer.wizards.com/Pages/Card/Languages.aspx?multiverseid={0}", id);
		}

		private void GetzID(Card card, LANGUAGE lang)
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

			if (lang == LANGUAGE.English || !webdata.Contains("This card is available in the following languages:") || !webdata.Contains(lang.ToString().Replace("_", " ")))
			{
				card.zID = string.Empty;
			}
			else
			{
				try
				{
					webdata = webdata.Remove(webdata.IndexOf(lang.ToString().Replace("_", " ")));
					int num = webdata.LastIndexOf("multiverseid=") + 13;
					int num2 = webdata.IndexOf("\"", num);
					card.zID = webdata.Substring(num, num2 - num);
				}
				catch (Exception ex)
				{
					throw new ParsingXception("Parsing Error happended when parsing card zID:" + lang.ToString(), ex);
				}
			}
		}
	}
}