using System;
using System.IO;
using System.Net;
using HyperKore.Exception;

namespace HyperPlugin.Net
{
	public class Downloader : IDownloader
	{
		#region IDownloader Members

		/// <summary>
		///     Downlaod file from the provided url and store it to the provided path
		/// </summary>
		/// <param name="url">DownloadBytes link</param>
		/// <param name="path">Stroring path</param>
		/// <param name="maxTryTimes"></param>
		public void Download(string url, string path, int maxTryTimes = 10)
		{
			//Max try times
			int tryCount = maxTryTimes;

			while (tryCount > 0)
			{
				try
				{
					using (var webClient = new WebClient())
					{
						webClient.Headers.Add("User_Agent", "Chrome");
						webClient.DownloadFile(url, path);
						break;
					}
				}
				catch (Exception ex)
				{
					//If time-out, retry
					if (ex is WebException && (ex as WebException).Status == WebExceptionStatus.Timeout)
					{
						tryCount--;
					}
					else
					{
						throw new DownloadingException();
					}
				}
			}
		}

		/// <summary>
		///     Download file into byte array
		/// </summary>
		/// <param name="url"></param>
		/// <param name="maxTryTimes"></param>
		/// <returns></returns>
		public byte[] Download(string url, int maxTryTimes = 10)
		{
			//Max try times
			int tryCount = maxTryTimes;
			byte[] data = null;

			while (tryCount > 0)
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
							data = memoryStream.GetBuffer();
							res.Close();
							memoryStream.Dispose();
							break;
						}
					}
				}
				catch (Exception ex)
				{
					//If time-out, retry
					if (ex is WebException && (ex as WebException).Status == WebExceptionStatus.Timeout)
					{
						tryCount--;
					}
					else
					{
						throw new DownloadingException();
					}
				}
			}

			return data;
		}

		#endregion

		#region Implementation of IPlugin

		public string Description { get; private set; }
		public string Name { get; private set; }

		#endregion
	}
}