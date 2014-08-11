using System.Collections.Generic;
using HyperKore.Common;

namespace HyperPlugin
{
	public interface IDataParse : IPlugin
	{

		/// <summary>
		///     Get set list in string format
		/// </summary>
		/// <returns></returns>
		IEnumerable<Set> ParseSet();

		/// <summary>
		///     Get a list of cards with all properties filled
		/// </summary>
		/// <param name="set"></param>
		/// <param name="lang"></param>
		/// <returns></returns>
		IEnumerable<Card> Process(Set set, Language lang = Language.English);

		/// <summary>
		///     Fill card properties
		/// </summary>
		/// <param name="card"></param>
		/// <param name="lang"></param>
		/// <returns>If card is not found, false will be returned</returns>
		bool Process(Card card, Language lang = Language.English);
	}
}