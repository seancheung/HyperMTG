namespace HyperPlugin
{
	public interface IRequest : IPlugin
	{
		/// <summary>
		///     Get Data from the provided url
		/// </summary>
		/// <param name="url">A url to create request</param>
		/// <param name="maxTryTimes">Max try times</param>
		/// <returns>Data from the response</returns>
		string GetWebData(string url, int maxTryTimes = 10);
	}
}