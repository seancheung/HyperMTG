using System.IO;
using HyperKore.Common;

namespace HyperPlugin
{
	public interface IDeckWriter : IPlugin
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
		///     Save deck to stream
		/// </summary>
		/// <param name="deck"></param>
		/// <param name="output"></param>
		/// <returns></returns>
		bool Write(Deck deck, Stream output);
	}
}