using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using HyperKore.Plugin;

namespace HyperKore.IO
{
	public class IOHandler
	{
		public static readonly IOHandler Instance = new IOHandler();

		private ICollection<ICompressor> compressors;

		private ICollection<IDBReader> dbReaders;

		private ICollection<IDBWriter> dbWriters;

		private ICollection<IDeckReader> deckReaders;

		private ICollection<IDeckWriter> deckWriters;

		/// <summary>
		///     Initializes a new instance of the IOHandler class.
		/// </summary>
		private IOHandler()
		{
			dbReaders = new List<IDBReader>();
			dbWriters = new List<IDBWriter>();
			deckReaders = new List<IDeckReader>();
			deckWriters = new List<IDeckWriter>();
			compressors = new List<ICompressor>();

			LoadPlugins(Directory.GetCurrentDirectory());
		}

		/// <summary>
		///     Get an instance of a plugin
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns>If plugin is not found, NULL will be returned</returns>
		public IEnumerable<T> GetPlugins<T>() where T : IPlugin
		{
			Type type = typeof (T);
			if (type == typeof (IDBReader))
			{
				return dbReaders as IEnumerable<T>;
			}
			if (type == typeof (IDBWriter))
			{
				return dbWriters as IEnumerable<T>;
			}
			if (type == typeof (IDeckReader))
			{
				return deckReaders as IEnumerable<T>;
			}
			if (type == typeof (IDeckWriter))
			{
				return deckWriters as IEnumerable<T>;
			}
			if (type == typeof (ICompressor))
			{
				return compressors as IEnumerable<T>;
			}
			return null;
		}

		[Conditional("DEBUG")]
		private void CountPlugins()
		{
			Debug.WriteLine("{0}:{1}", typeof (IDBReader), dbReaders.Count);
			Debug.WriteLine("{0}:{1}", typeof (IDBWriter), dbReaders.Count);
			Debug.WriteLine("{0}:{1}", typeof (IDeckReader), deckReaders.Count);
			Debug.WriteLine("{0}:{1}", typeof (IDeckWriter), deckWriters.Count);
			Debug.WriteLine("{0}:{1}", typeof (ICompressor), compressors.Count);
		}

		private void LoadPlugins(string path)
		{
			dbReaders = PluginLoader<IDBReader>.Load(path);
			dbWriters = PluginLoader<IDBWriter>.Load(path);
			deckReaders = PluginLoader<IDeckReader>.Load(path);
			deckWriters = PluginLoader<IDeckWriter>.Load(path);
			compressors = PluginLoader<ICompressor>.Load(path);

			CountPlugins();
		}
	}
}