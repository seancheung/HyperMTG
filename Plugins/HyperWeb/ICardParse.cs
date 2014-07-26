using HyperKore.Common;

namespace HyperPlugin.Web
{
	public interface ICardParse
	{
		/// <summary>
		/// Parse the card
		/// </summary>
		/// <param name="card"></param>
		/// <param name="lang"></param>
		void Parse(Card card, LANGUAGE lang);
	}
}