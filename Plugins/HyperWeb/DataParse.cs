using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using HyperKore.Common;
using HyperKore.Exception;
using HyperKore.Utilities;
using ScrapySharp.Extensions;

namespace HyperPlugin
{
	public class DataParse : IDataParse, IImageParse
	{
		private readonly IDownloader downloader;
		private readonly IRequest request;

		public DataParse()
		{
			request = PluginManager.Instance.GetPlugin<IRequest>();
			downloader = PluginManager.Instance.GetPlugin<IDownloader>();
			if (request == null || downloader == null)
			{
				throw new AssamlyMissingException();
			}
		}

		/// <summary>
		///     Get url of the card list data
		/// </summary>
		/// <param name="setcode">Full english set name</param>
		/// <returns>the url for webrequesting</returns>
		private string BuildURL(string setcode, Language lang)
		{
			return string.Format("http://magiccards.info/query?q=%2B%2Be%3A{0}%2F{1}&v=spoiler&s=issue", setcode.ToLower(),
				lang.GetLangCode());
		}

		#region Implementation of IPlugin

		#region IDataParse Members

		public string Description { get; private set; }
		public string Name { get; private set; }

		#endregion

		#region IImageParse Members

		/// <summary>
		///     Download file into byte array
		/// </summary>
		/// <param name="card"></param>
		/// <param name="lang"></param>
		/// <returns></returns>
		public byte[] Download(Card card, Language lang = Language.English)
		{
			string uri = string.Format("http://magiccards.info/scans/{0}/{1}/{2}.jpg", lang.GetLangCode(), card.SetCode.ToLower(),
				card.Number);
			return downloader.Download(uri);
		}

		#endregion

		#endregion

		#region Implementation of IDataParse

		/// <summary>
		///     Get set list in string format
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Set> ParseSet()
		{
			string webdata = request.GetWebData(@"http://magiccards.info/search.html");
			if (string.IsNullOrWhiteSpace(webdata))
			{
				yield break;
			}

			HtmlDocument html = new HtmlDocument();
			html.LoadHtml(webdata);

			string select =
				html.DocumentNode.CssSelect("select#edition").First().InnerHtml;

			Match matchSC = Regex.Match(select, @"(?<=<option\svalue="")(\w+)(?=/(\w+)"">)");
			Match matchSN = Regex.Match(select, @"(?<="">)(?!\s|<|>)[\w\s\:\'\""\.\/\-]+(?=<)");

			while (matchSC.Success && matchSN.Success)
			{
				yield return
					new Set { SetName = matchSN.Value.Trim(), SetCode = matchSC.Value.Trim().ToUpper() };

				matchSC = matchSC.NextMatch();
				matchSN = matchSN.NextMatch();
			}
		}

		/// <summary>
		///     Get a list of cards with all properties filled
		/// </summary>
		/// <param name="set"></param>
		/// <param name="lang"></param>
		/// <returns></returns>
		public IEnumerable<Card> Process(Set set, Language lang = Language.English)
		{
			string webdata = request.GetWebData(BuildURL(set.SetCode, lang));
			if (webdata.Contains("Your query did not match any cards"))
			{
				yield break;
			}

			HtmlDocument html = new HtmlDocument();
			html.LoadHtml(webdata);

			IEnumerable<HtmlNode> tds = html.DocumentNode.CssSelect("td").Where(t => t.Attributes["valign"] != null);

			foreach (HtmlNode td in tds)
			{
				Card card = new Card { Set = set.SetName, SetCode = set.SetCode };

				HtmlNode span = td.CssSelect("span").FirstOrDefault();
				if (span == null)
				{
					continue;
				}
				card.Name = span.InnerText.Trim();
				string id = span.CssSelect("a").First().Attributes["href"].Value.Replace(".html", "");
				id = Regex.Replace(id, @"^/|\.html$", "", RegexOptions.IgnoreCase);

				HtmlNode[] ps = td.CssSelect("p").ToArray();
				if (ps.Length != 5)
				{
					continue;
				}
				card.Rarity = ps[0].InnerText.Split(',')[1].Trim();

				string[] typCos = ps[1].InnerText.Split(',');
				if (typCos.Length < 2)
				{
					continue;
				}
				string type = typCos[0].Trim();
				card.Loyalty = Regex.Match(type, @"(?<=\(Loyalty:\s*)(\d+)(?=\))").Value;
				string pt = Regex.Match(type, @"(?<=\s)(\d+|\*)/(\d+|\*)").Value;
				if (!string.IsNullOrWhiteSpace(pt))
				{
					card.Pow = pt.Split('/')[0];
					card.Tgh = pt.Split('/')[1];
					type = type.Replace(pt, "").Trim();
				}
				if (!string.IsNullOrWhiteSpace(card.Loyalty))
				{
					type = Regex.Replace(type, @"\(.*\)", "").Trim();
				}
				else
				{
					card.Loyalty = null;
				}
				card.Type = type;

				string cost = typCos[1].Trim();
				if (!string.IsNullOrWhiteSpace(cost))
				{
					card.Cost = Regex.Split(cost, @"\s")[0].ManaBuild();

					card.CMC = Regex.Match(cost, @"(?<=\()\d+(?=\))").Value.Trim();
				}
				else
				{
					card.Cost = null;
					card.CMC = "0";
				}
				card.Text = ps[2].InnerHtml.RemoveHtmlTag().ManaFormat();
				card.Flavor = ps[3].InnerHtml.RemoveHtmlTag();
				card.Artist = ps[4].InnerText.Trim();
				card.Number = Regex.Match(id, @"(?<=/)\w+\z").Value;

				card.ID = string.Format("{0}#{1}", card.SetCode, card.Number);

				yield return card;
			}
		}

		/// <summary>
		///     Fill card properties
		/// </summary>
		/// <param name="card"></param>
		/// <param name="lang"></param>
		/// <returns>If card is not found, false will be returned</returns>
		public bool Process(Card card, Language lang = Language.English)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}