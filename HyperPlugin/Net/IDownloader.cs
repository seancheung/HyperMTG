namespace HyperPlugin
{
	public interface IDownloader : IPlugin
	{
		/// <summary>
		///     Downlaod file from the provided url and store it to the provided path
		/// </summary>
		/// <param name="url">DownloadBytes link</param>
		/// <param name="path">Stroring path</param>
		/// <param name="maxTryTimes"></param>
		void Download(string url, string path, int maxTryTimes = 10);

		/// <summary>
		///     Download file into byte array
		/// </summary>
		/// <param name="url"></param>
		/// <param name="maxTryTimes"></param>
		/// <returns></returns>
		byte[] Download(string url, int maxTryTimes = 10);
	}
}