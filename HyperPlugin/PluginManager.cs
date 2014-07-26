using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HyperPlugin
{
	public class PluginManager
	{
		/// <summary>
		///     Single Instance
		/// </summary>
		public static readonly PluginManager Instance = new PluginManager();

		private PluginManager()
		{
		}

		#region Members

		private IEnumerable<ICompressor> compressors;
		private IEnumerable<IDataParse> dataParses;
		private IEnumerable<IDBReader> dbReaders;
		private IEnumerable<IDBWriter> dbWriters;
		private IEnumerable<IDeckReader> deckReaders;
		private IEnumerable<IDeckWriter> deckWriters;
		private IEnumerable<IDownloader> downloaders;
		private IEnumerable<IImageParse> imageParses;
		private IEnumerable<IRequest> requests;

		#endregion

		/// <summary>
		///     Get all plugins of target type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns>If no plugins are found, an empty list will be returned</returns>
		public IEnumerable<T> GetPlugins<T>() where T : IPlugin
		{
			string path = Directory.GetCurrentDirectory();
			Type type = typeof (T);
			if (type == typeof (IDBReader))
			{
				return (dbReaders ?? (dbReaders = PluginLoader<IDBReader>.Load(path))) as IEnumerable<T>;
			}

			if (type == typeof (IDBWriter))
			{
				return (dbWriters ?? (dbWriters = PluginLoader<IDBWriter>.Load(path))) as IEnumerable<T>;
			}

			if (type == typeof (IDeckReader))
			{
				return (deckReaders ?? (deckReaders = PluginLoader<IDeckReader>.Load(path))) as IEnumerable<T>;
			}

			if (type == typeof (IDeckWriter))
			{
				return (deckWriters ?? (deckWriters = PluginLoader<IDeckWriter>.Load(path))) as IEnumerable<T>;
			}

			if (type == typeof (IDownloader))
			{
				return (downloaders ?? (downloaders = PluginLoader<IDownloader>.Load(path))) as IEnumerable<T>;
			}

			if (type == typeof (IImageParse))
			{
				return (imageParses ?? (imageParses = PluginLoader<IImageParse>.Load(path))) as IEnumerable<T>;
			}

			if (type == typeof (IRequest))
			{
				return (requests ?? (requests = PluginLoader<IRequest>.Load(path))) as IEnumerable<T>;
			}

			if (type == typeof (IDataParse))
			{
				return (dataParses ?? (dataParses = PluginLoader<IDataParse>.Load(path))) as IEnumerable<T>;
			}

			if (type == typeof (ICompressor))
			{
				return (compressors ?? (compressors = PluginLoader<ICompressor>.Load(path))) as IEnumerable<T>;
			}

			return new List<T>();
		}

		/// <summary>
		///     Get the default plugin of target type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns>If no plugins are found, Null will be returned</returns>
		public T GetPlugin<T>() where T : IPlugin
		{
			return GetPlugins<T>().FirstOrDefault();
		}
	}
}