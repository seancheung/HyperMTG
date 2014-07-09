using HyperKore.Common;
using HyperKore.Plugin;
using System.Collections.Generic;

namespace HyperKore.IO
{
	public interface IDBWriter : IPlugin
	{
		/// <summary>
		/// Type of the Database
		/// </summary>
		string DBType { get; }

		/// <summary>
		/// Dlete card
		/// </summary>
		/// <param name="card"></param>
		/// <returns></returns>
		bool Delete(Card card);

		/// <summary>
		/// Delte file by id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		bool Delete(string id);

		/// <summary>
		/// Insert card
		/// </summary>
		/// <param name="card"></param>
		/// <returns></returns>
		bool Insert(Card card);

		/// <summary>
		/// Insert cards
		/// </summary>
		/// <param name="cards"></param>
		void Insert(IEnumerable<Card> cards);

		/// <summary>
		/// Insert sets
		/// </summary>
		/// <param name="sets"></param>
		void Insert(IEnumerable<Set> sets);

		/// <summary>
		/// Insert file
		/// </summary>
		/// <param name="id"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		bool Insert(string id, byte[] data, ICompressor compressor);

		/// <summary>
		/// Update card
		/// </summary>
		/// <param name="card"></param>
		/// <returns></returns>
		bool Update(Card card);

		/// <summary>
		/// Update file
		/// </summary>
		/// <param name="id"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		bool Update(string id, byte[] data, ICompressor compressor);

		/// <summary>
		/// Update set
		/// </summary>
		/// <param name="set"></param>
		/// <returns></returns>
		bool Update(Set set);
	}
}