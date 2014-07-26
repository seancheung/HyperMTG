namespace HyperPlugin
{
	public interface IDownloader : IPlugin
	{
		/// <summary>
		///     Downlaod file from the provided url and store it to the provided path
		/// </summary>
		/// <param name="url">DownloadBytes link</param>
		/// <param name="path">Stroring path</param>
		void Download(string url, string path);

		/// <summary>
		///     Download file into byte array
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		byte[] Download(string url);
	}
}