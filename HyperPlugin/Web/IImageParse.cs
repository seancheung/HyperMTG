using HyperKore.Common;

namespace HyperPlugin
{
	public interface IImageParse : IPlugin
	{
		/// <summary>
		///     Download file into byte array
		/// </summary>
		/// <param name="card"></param>
		/// <param name="lang"></param>
		/// <returns></returns>
		byte[] Download(Card card, Language lang = Language.English);
	}
}