using System;
using System.IO;
using System.Linq;
using System.Reflection;
using HyperPlugin;

namespace HyperMTG.ViewModel
{
	public class SealedViewModel
	{
		private readonly ICompressor _compressor;

		private readonly IDBReader _dbReader;
		private readonly IDBWriter _dbWriter;
		private readonly IDeckReader[] _deckReaders;
		private readonly IDeckWriter[] _deckWriters;
		private readonly IImageParse _imageParse;

		public SealedViewModel()
		{
			try
			{
				_dbReader = PluginManager.Instance.GetPlugin<IDBReader>();
				_dbWriter = PluginManager.Instance.GetPlugin<IDBWriter>();
				_compressor = PluginManager.Instance.GetPlugin<ICompressor>();
				_imageParse = PluginManager.Instance.GetPlugin<IImageParse>();
				_deckWriters = PluginManager.Instance.GetPlugins<IDeckWriter>().ToArray();
				_deckReaders = PluginManager.Instance.GetPlugins<IDeckReader>().ToArray();
			}
			catch (ReflectionTypeLoadException e)
			{
				foreach (var exception in e.LoaderExceptions)
				{
					File.AppendAllText("error.log", exception.Message);
				}
			}
		}
	}
}