namespace HyperPlugin
{
	public interface IImageParse : IPlugin
	{
		/// <summary>
		///     Download file into byte array
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		byte[] Download(string id);
	}
}