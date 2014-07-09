using HyperKore.Xception;
using System;
using System.IO;
using System.Net;

namespace HyperKore.Net
{
	public class Request
	{
		/// <summary>
		/// Single Instance
		/// </summary>
		public static readonly Request Instance = new Request();

		private Request()
		{
		}

		/// <summary>
		/// Get Data from the provided url
		/// </summary>
		/// <param name="url">A url to create request</param>
		/// <param name="maxTryTimes">Max try times</param>
		/// <returns>Data from the response</returns>
		public string GetWebData(string url, int maxTryTimes = 10)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.AllowAutoRedirect = false;
			string data = string.Empty;

			//Max try times
			int tryCount = maxTryTimes;

			while (tryCount > 0)
			{
				try
				{
					using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
					{
						if (!httpWebResponse.StatusDescription.Equals("Found"))
						{
							StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
							data = streamReader.ReadToEnd();
						}
					}
					break;
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
						throw new RequestXception(tryCount, url, "Requesting Error", ex);
					}
				}
			}

			return data;
		}
	}
}