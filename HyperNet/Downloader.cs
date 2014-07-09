using HyperKore.Xception;
using System;
using System.IO;
using System.Net;

namespace HyperKore.Net
{
	public class Downloader
	{
		/// <summary>
		/// Single Instance
		/// </summary>
		public static readonly Downloader Instance = new Downloader();

		private Downloader()
		{
		}

		/// <summary>
		/// Downlaod file from the provided url and store it to the provided path
		/// </summary>
		/// <param name="url">DownloadBytes link</param>
		/// <param name="path">Stroring path</param>
		public void Download(string url, string path)
		{
			try
			{
				using (WebClient webClient = new WebClient())
				{
					webClient.Headers.Add("User_Agent", "Chrome");
					webClient.DownloadFile(url, path);
				}
			}
			catch (Exception ex)
			{
				throw new DownloadingXception(url, path, "Downloading File Error", ex);
			}
		}

		/// <summary>
		/// Download file into byte array
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public byte[] Download(string url)
		{
			try
			{
				HttpWebRequest req = HttpWebRequest.Create(url) as HttpWebRequest;
				req.AllowAutoRedirect = true;
				//req.Referer = "";

				req.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; zh-CN; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13";

				HttpWebResponse res = req.GetResponse() as HttpWebResponse;

				Stream stream = res.GetResponseStream();
				MemoryStream memoryStream = new MemoryStream();
				byte[] buffer = new byte[32 * 1024];
				int bytes;
				while ((bytes = stream.Read(buffer, 0, buffer.Length)) > 0)
				{
					memoryStream.Write(buffer, 0, bytes);
				}
				byte[] data = memoryStream.GetBuffer();
				res.Close();
				memoryStream.Dispose();
				return data;
			}
			catch (Exception ex)
			{
				throw new DownloadingXception(url, "Caching File Error", ex);
			}
		}
	}
}