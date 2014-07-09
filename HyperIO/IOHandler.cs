using HyperKore.Plugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HyperKore.IO
{
	public class IOHandler
	{
		public static readonly IOHandler Instance = new IOHandler();

		private ICollection<ICompressor> Compressors;

		private ICollection<IDBReader> DBReaders;

		private ICollection<IDBWriter> DBWriters;

		private ICollection<IDeckReader> DeckReaders;

		private ICollection<IDeckWriter> DeckWriters;

		/// <summary>
		/// Initializes a new instance of the IOHandler class.
		/// </summary>
		private IOHandler()
		{
			DBReaders = new List<IDBReader>();
			DBWriters = new List<IDBWriter>();
			DeckReaders = new List<IDeckReader>();
			DeckWriters = new List<IDeckWriter>();
			Compressors = new List<ICompressor>();

			LoadPlugins(System.IO.Directory.GetCurrentDirectory());
		}

		/// <summary>
		/// Get an instance of a plugin
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns>If plugin is not found, NULL will be returned</returns>
		public IEnumerable<IPlugin> GetPlugins<T>() where T : IPlugin
		{
			var type = typeof(T);
			if (type == typeof(IDBReader))
			{
				return DBReaders;
			}
			else if (type == typeof(IDBWriter))
			{
				return DBWriters;
			}
			else if (type == typeof(IDeckReader))
			{
				return DeckReaders;
			}
			else if (type == typeof(IDeckWriter))
			{
				return DeckWriters;
			}
			else if (type == typeof(ICompressor))
			{
				return Compressors;
			}
			else
			{
				return null;
			}
		}

		[Conditional("DEBUG")]
		private void CountPlugins()
		{
			Debug.WriteLine(String.Format("{0}:{1}", typeof(IDBReader), DBReaders.Count));
			Debug.WriteLine(String.Format("{0}:{1}", typeof(IDBWriter), DBReaders.Count));
			Debug.WriteLine(String.Format("{0}:{1}", typeof(IDeckReader), DeckReaders.Count));
			Debug.WriteLine(String.Format("{0}:{1}", typeof(IDeckWriter), DeckWriters.Count));
			Debug.WriteLine(String.Format("{0}:{1}", typeof(ICompressor), Compressors.Count));
		}

		private void LoadPlugins(string path)
		{
			DBReaders = PluginLoader<IDBReader>.Load(path);
			DBWriters = PluginLoader<IDBWriter>.Load(path);
			DeckReaders = PluginLoader<IDeckReader>.Load(path);
			DeckWriters = PluginLoader<IDeckWriter>.Load(path);
			Compressors = PluginLoader<ICompressor>.Load(path);

			CountPlugins();
		}
	}
}