using System.Collections.Generic;
using HyperKore.Common;

namespace HyperPlugin
{
	public interface IDBReader : IPlugin
	{
		/// <summary>
		///     Type of the Database
		/// </summary>
		string DBType { get; }

		/// <summary>
		///     Language of the database
		/// </summary>
		Language Language { get; set; }

		/// <summary>
		///     Load cards
		/// </summary>
		/// <returns></returns>
		IEnumerable<Card> LoadCards();

		/// <summary>
		///     Load file
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		byte[] LoadFile(string id, ICompressor compressor);

		/// <summary>
		///     Check card images in database then return those which are not found
		/// </summary>
		/// <param name="cards"></param>
		/// <returns></returns>
		IEnumerable<Card> CheckFiles(IEnumerable<Card> cards);

		/// <summary>
		///     Load sets
		/// </summary>
		/// <returns></returns>
		IEnumerable<Set> LoadSets();
	}
}