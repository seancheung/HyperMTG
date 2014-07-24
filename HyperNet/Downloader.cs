using System;
using System.IO;
using System.Net;
using HyperKore.Xception;

namespace HyperKore.Net
{
	public class Downloader
	{
		/// <summary>
		///     Single Instance
		/// </summary>
		public static readonly Downloader Instance = new Downloader();

		private Downloader()
		{
		}

		/// <summary>
		///     Downlaod file from the provided url and store it to the provided path
		/// </summary>
		/// <param name="url">DownloadBytes link</param>
		/// <param name="path">Stroring path</param>
		public void Download(string url, string path)
		{
			try
			{
				using (var webClient = new WebClient())
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
		///     Download file into byte array
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public byte[] Download(string url)
		{
			try
			{
				var req = WebRequest.Create(url) as HttpWebRequest;
				if (req != null)
				{
					req.AllowAutoRedirect = true;
					//req.Referer = "";

					req.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; zh-CN; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13";

					var res = req.GetResponse() as HttpWebResponse;

					if (res != null)
					{
						Stream stream = res.GetResponseStream();
						var memoryStream = new MemoryStream();
						var buffer = new byte[32*1024];
						int bytes;
						while (stream != null && (bytes = stream.Read(buffer, 0, buffer.Length)) > 0)
						{
							memoryStream.Write(buffer, 0, bytes);
						}
						byte[] data = memoryStream.GetBuffer();
						res.Close();
						memoryStream.Dispose();
						return data;
					}
				}
			}
			catch (Exception ex)
			{
				throw new DownloadingXception(url, "Caching File Error", ex);
			}
			return null;
		}
	}
}