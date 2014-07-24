using System;
using System.IO;
using System.Net;
using HyperKore.Common;
using HyperKore.Net;
using HyperKore.Utilities;
using HyperKore.Xception;

namespace HyperKore.Web
{
	public class ImageParse
	{
		public static readonly ImageParse Instance = new ImageParse();

		private ImageParse()
		{
		}

		/// <summary>
		///     Download file into byte array
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public byte[] Download(string id)
		{
			string url = GetURL(id, null, null);
			return Downloader.Instance.Download(url);
		}

		/// <summary>
		/// Get image downloading url
		/// </summary>
		/// <param name="id"></param>
		/// <param name="setcode"></param>
		/// <param name="num"></param>
		/// <param name="lang"></param>
		/// <param name="site"></param>
		/// <returns></returns>
		private string GetURL(string id, string setcode, string num, LANGUAGE lang = LANGUAGE.English, WEBSITE site = WEBSITE.gatherer)
		{
			//Default is gatherer
			string result = string.Format("http://gatherer.wizards.com/Handlers/Image.ashx?multiverseid={0}&type=card", id);

			switch (site)
			{
				case WEBSITE.gatherer:
					break;
				case WEBSITE.magiccards:
					result = string.Format("http://magiccards.info/scans/{0}/{1}/{2}.jpg", lang.GetLangCode(), setcode.ToLower(), num);
					break;
				case WEBSITE.magicspoiler:
					break;
				case WEBSITE.iplaymtg:
					result = string.Format("http://data.iplaymtg.com/mtgdeck/card/{0}/{1}/{2}.jpg", lang.GetLangCode(), setcode.ToUpper(), num);
					break;
			}

			return result;

		}
	}
}