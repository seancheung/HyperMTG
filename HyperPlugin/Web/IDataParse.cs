using System.Collections.Generic;
using HyperKore.Common;

namespace HyperPlugin
{
	public interface IDataParse : IPlugin
	{
		/// <summary>
		///     Get format list
		/// </summary>
		/// <returns></returns>
		IEnumerable<string> ParseFormat();

		/// <summary>
		///     Get set list in string format
		/// </summary>
		/// <returns></returns>
		IEnumerable<string> ParseSet();

		/// <summary>
		///     Get set list
		/// </summary>
		/// <returns></returns>
		IEnumerable<Set> ParSetWithCode();

		/// <summary>
		///     Get type list
		/// </summary>
		/// <returns></returns>
		IEnumerable<string> ParseType();

		/// <summary>
		///     Get a list of cards with ID property filled
		/// </summary>
		/// <param name="set"></param>
		/// <returns></returns>
		IEnumerable<Card> Prepare(Set set);

		/// <summary>
		///     Get a list of cards with all properties filled
		/// </summary>
		/// <param name="set"></param>
		/// <param name="lang"></param>
		/// <returns></returns>
		IEnumerable<Card> PrepareAndProcess(Set set, LANGUAGE lang);

		/// <summary>
		///     Fill card properties
		/// </summary>
		/// <param name="card"></param>
		/// <param name="lang"></param>
		/// <returns>If card is not found, false will be returned</returns>
		bool Process(Card card, LANGUAGE lang = LANGUAGE.English);
	}
}