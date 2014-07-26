using System.Collections.Generic;
using System.IO;
using HyperKore.Common;

namespace HyperPlugin
{
	public interface IDeckReader : IPlugin
	{
		/// <summary>
		///     File extension
		/// </summary>
		string FileExt { get; }

		/// <summary>
		///     Type of the deck
		/// </summary>
		string DeckType { get; }

		/// <summary>
		///     Load deck from stream
		/// </summary>
		/// <param name="input"></param>
		/// <param name="database"></param>
		/// <returns></returns>
		Deck Read(Stream input, IEnumerable<Card> database);
	}
}