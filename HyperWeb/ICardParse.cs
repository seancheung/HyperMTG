using HyperKore.Common;

namespace HyperKore.Web
{
	public interface ICardParse
	{
		/// <summary>
		/// Parse the card
		/// </summary>
		/// <param name="card"></param>
		void Parse(Card card, LANGUAGE lang);
	}
}