using System;
using System.IO;
using System.Net;
using HyperKore.Exception;

namespace HyperPlugin
{
	public class Request : IRequest
	{
		#region IRequest Members

		/// <summary>
		///     Get Data from the provided url
		/// </summary>
		/// <param name="url">A url to create request</param>
		/// <param name="maxTryTimes">Max try times</param>
		/// <returns>Data from the response</returns>
		public string GetWebData(string url, int maxTryTimes = 10)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(url);
			httpWebRequest.AllowAutoRedirect = false;
			string data = string.Empty;

			//Max try times
			int tryCount = maxTryTimes;

			while (tryCount > 0)
			{
				try
				{
					using (HttpWebResponse httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse())
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
						throw new RequestException();
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