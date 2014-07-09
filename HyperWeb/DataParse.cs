using HyperKore.Common;
using HyperKore.Net;
using HyperKore.Xception;
using System;
using System.Collections.Generic;

namespace HyperKore.Web
{
	public class DataParse
	{
		/// <summary>
		/// Single Instance
		/// </summary>
		public static readonly DataParse Instance = new DataParse();

		private static ICardParse[] parse;

		private DataParse()
		{
			parse = new ICardParse[5];
			parse[0] = new ParseDetail();
			parse[1] = new ParsezID();
			parse[2] = new ParsezDetail();
			parse[3] = new ParseLegality();
			parse[4] = new ParseEx();
		}

		/// <summary>
		/// Get format list
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> ParseFormat()
		{
			return new ParseOther().ParseFormat();
		}

		/// <summary>
		/// Get set list
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> ParseSet()
		{
			return new ParseOther().ParseSet();
		}

		/// <summary>
		/// Get type list
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> ParseType()
		{
			return new ParseOther().ParseType();
		}

		/// <summary>
		/// Get a list of cards with ID property filled
		/// </summary>
		/// <param name="set"></param>
		/// <returns></returns>
		public IEnumerable<Card> Prepare(Set set)
		{
			string webdata;
			try
			{
				webdata = Request.Instance.GetWebData(BuildURL(set.SetName));
			}
			catch
			{
				throw;
			}

			if (webdata.Contains("Your search returned zero results"))
			{
				yield break;
			}

			while (webdata.Contains("multiverseid="))
			{
				Card card = new Card();
				try
				{
					var numa = webdata.IndexOf("\"number\"") + 9;
					var numb = webdata.IndexOf("<", numa);
					card.Number = webdata.Substring(numa, numb - numa);

					var ida = webdata.IndexOf("multiverseid=") + 13;
					var idb = webdata.IndexOf("\"", ida);
					card.ID = webdata.Substring(ida, idb - ida);

					var namea = webdata.IndexOf(">", idb) + 1;
					var nameb = webdata.IndexOf("<", namea);
					card.Name = webdata.Substring(namea, nameb - namea);

					var arta = webdata.IndexOf("\"artist\"") + 9;
					var artb = webdata.IndexOf("<", arta);
					card.Artist = webdata.Substring(arta, artb - arta);

					var coa = webdata.IndexOf("\"color\"") + 8;
					var cob = webdata.IndexOf("<", coa);
					card.Color = webdata.Substring(coa, cob - coa).Replace("/", " ");

					var rca = webdata.IndexOf("\"rarity\"") + 9;
					var rcb = webdata.IndexOf("<", rca);
					card.RarityCode = webdata.Substring(rca, rcb - rca).Replace("/", " ");

					card.Set = set.SetName;
					card.SetCode = set.SetCode;

					webdata = webdata.Substring(idb);
				}
				catch (Exception ex)
				{
					throw new ParsingXception("Parsing Error happended when fetching ID list", ex);
				}

				yield return card;
			}
		}

		/// <summary>
		///  Get a list of cards with all properties filled
		/// </summary>
		/// <param name="set"></param>
		/// <param name="lang"></param>
		/// <returns></returns>
		public IEnumerable<Card> PrepareAndProcess(Set set, LANGUAGE lang)
		{
			var cards = Prepare(set);

			foreach (var card in cards)
			{
				var state = Process(card, lang);
				if (state)
				{
					yield return card;
				}
			}
		}

		/// <summary>
		/// Fill card properties
		/// </summary>
		/// <param name="card"></param>
		/// <param name="lang"></param>
		/// <returns>If card is not found, false will be returned</returns>
		public bool Process(Card card, LANGUAGE lang = LANGUAGE.English)
		{
			foreach (var p in parse)
			{
				p.Parse(card, lang);
				if (card == null) return false;
			}

			return true;
		}

		/// <summary>
		/// Get url of the card list data
		/// </summary>
		/// <param name="setname">Full english set name</param>
		/// <returns>the url for webrequesting</returns>
		private string BuildURL(string setname)
		{
			return string.Format("http://gatherer.wizards.com/Pages/Search/Default.aspx?output=checklist&set=%5b%22{0}%22%5d", setname.Replace(" ", "+"));
		}
	}
}