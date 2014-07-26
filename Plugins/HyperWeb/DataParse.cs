using System;
using System.Collections.Generic;
using System.Linq;
using HyperKore.Common;
using HyperKore.Exception;

namespace HyperPlugin.Web
{
	public class DataParse : IDataParse
	{
		private readonly ICardParse[] parse;
		private readonly IRequest request;

		public DataParse()
		{
			parse = new ICardParse[5];
			parse[0] = new ParseDetail();
			parse[1] = new ParsezID();
			parse[2] = new ParsezDetail();
			parse[3] = new ParseLegality();
			parse[4] = new ParseEx();

			request = PluginManager.Instance.GetPlugin<IRequest>();
			if (request == null)
			{
				throw new AssamlyMissingException();
			}
		}

		#region IDataParse Members

		/// <summary>
		///     Get format list
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> ParseFormat()
		{
			return new ParseOther().ParseFormat();
		}

		/// <summary>
		///     Get set list in string format
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> ParseSet()
		{
			return new ParseOther().ParseSet();
		}

		/// <summary>
		///     Get set list
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Set> ParSetWithCode()
		{
			return new ParseOther().ParseSetWithCode();
		}

		/// <summary>
		///     Get type list
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> ParseType()
		{
			return new ParseOther().ParseType();
		}

		/// <summary>
		///     Get a list of cards with ID property filled
		/// </summary>
		/// <param name="set"></param>
		/// <returns></returns>
		public IEnumerable<Card> Prepare(Set set)
		{
			string webdata = request.GetWebData(BuildURL(set.SetName));

			if (webdata.Contains("Your search returned zero results"))
			{
				yield break;
			}

			while (webdata.Contains("multiverseid="))
			{
				var card = new Card();
				try
				{
					int numa = webdata.IndexOf("\"number\"") + 9;
					int numb = webdata.IndexOf("<", numa);
					card.Number = webdata.Substring(numa, numb - numa);

					int ida = webdata.IndexOf("multiverseid=") + 13;
					int idb = webdata.IndexOf("\"", ida);
					card.ID = webdata.Substring(ida, idb - ida);

					int namea = webdata.IndexOf(">", idb) + 1;
					int nameb = webdata.IndexOf("<", namea);
					card.Name = webdata.Substring(namea, nameb - namea);

					int arta = webdata.IndexOf("\"artist\"", nameb) + 9;
					int artb = webdata.IndexOf("<", arta);
					card.Artist = webdata.Substring(arta, artb - arta);

					int coa = webdata.IndexOf("\"color\"", artb) + 8;
					int cob = webdata.IndexOf("<", coa);
					card.Color = webdata.Substring(coa, cob - coa).Replace("/", " ");

					int rca = webdata.IndexOf("\"rarity\"", cob) + 9;
					int rcb = webdata.IndexOf("<", rca);
					card.RarityCode = webdata.Substring(rca, rcb - rca).Replace("/", " ");

					card.Set = set.SetName;
					card.SetCode = set.SetCode;

					webdata = webdata.Substring(idb);
				}
				catch (Exception ex)
				{
					throw new ParsingException();
				}

				yield return card;
			}
		}

		/// <summary>
		///     Get a list of cards with all properties filled
		/// </summary>
		/// <param name="set"></param>
		/// <param name="lang"></param>
		/// <returns></returns>
		public IEnumerable<Card> PrepareAndProcess(Set set, LANGUAGE lang)
		{
			IEnumerable<Card> cards = Prepare(set);

			return from card in cards let state = Process(card, lang) where state select card;
		}

		/// <summary>
		///     Fill card properties
		/// </summary>
		/// <param name="card"></param>
		/// <param name="lang"></param>
		/// <returns>If card is not found, false will be returned</returns>
		public bool Process(Card card, LANGUAGE lang = LANGUAGE.English)
		{
			foreach (ICardParse p in parse)
			{
				p.Parse(card, lang);
				if (card == null) return false;
			}

			return true;
		}

		#endregion

		/// <summary>
		///     Get url of the card list data
		/// </summary>
		/// <param name="setname">Full english set name</param>
		/// <returns>the url for webrequesting</returns>
		private string BuildURL(string setname)
		{
			return string.Format("http://gatherer.wizards.com/Pages/Search/Default.aspx?output=checklist&set=%5b%22{0}%22%5d",
				setname.Replace(" ", "+"));
		}

		#region Implementation of IPlugin

		public string Description { get; private set; }
		public string Name { get; private set; }

		#endregion
	}
}