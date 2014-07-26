using System;
using HyperKore.Common;
using HyperKore.Exception;
using HyperKore.Utilities;

namespace HyperPlugin.Web
{
	internal class ParsezID : ICardParse
	{
		private readonly IRequest request;

		public ParsezID()
		{
			request = PluginManager.Instance.GetPlugin<IRequest>();
			if (request == null)
			{
				throw new AssamlyMissingException();
			}
		}

		#region ICardParse Members

		public void Parse(Card card, LANGUAGE lang)
		{
			GetzID(card, lang);

			//use traditional chinese in case of simplified being unavailable
			if (card.zID == string.Empty && lang == LANGUAGE.ChineseSimplified)
			{
				GetzID(card, LANGUAGE.ChineseTraditional);
			}
		}

		#endregion

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
			string webdata = request.GetWebData(BuildURL(card.ID));

			if (lang == LANGUAGE.English || !webdata.Contains("This card is available in the following languages:") ||
			    !webdata.Contains(lang.GetLangName()))
			{
				card.zID = string.Empty;
			}
			else
			{
				try
				{
					webdata = webdata.Remove(webdata.IndexOf(lang.GetLangName()));
					int num = webdata.LastIndexOf("multiverseid=") + 13;
					int num2 = webdata.IndexOf("\"", num);
					card.zID = webdata.Substring(num, num2 - num);
				}
				catch (Exception ex)
				{
					throw new ParsingException();
				}
			}
		}
	}
}