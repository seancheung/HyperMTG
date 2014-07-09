using HyperKore.Common;
using HyperKore.Plugin;
using System.Collections.Generic;

namespace HyperKore.IO
{
	public interface IDBReader : IPlugin
	{
		/// <summary>
		/// Type of the Database
		/// </summary>
		string DBType { get; }

		/// <summary>
		/// Load cards
		/// </summary>
		/// <returns></returns>
		IEnumerable<Card> LoadCards();

		/// <summary>
		/// Load file
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		byte[] LoadFile(string id, ICompressor compressor);

		/// <summary>
		/// Load sets
		/// </summary>
		/// <returns></returns>
		IEnumerable<Set> LoadSets();
	}
}