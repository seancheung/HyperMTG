using System.Collections.Generic;
using System.Linq;
using HyperKore.Common;
using HyperPlugin;

namespace HyperMTGMain.Helper
{
	public class PluginFactory
	{
		public static IDBReader DbReader
		{
			get { return PluginManager.Instance.GetPlugin<IDBReader>(); }
		}

		public static IDBWriter DbWriter
		{
			get { return PluginManager.Instance.GetPlugin<IDBWriter>(); }
		}

		public static ICompressor Compressor
		{
			get { return PluginManager.Instance.GetPlugin<ICompressor>(); }
		}

		public static IImageParse ImageParse
		{
			get { return PluginManager.Instance.GetPlugin<IImageParse>(); }
		}

		public static IEnumerable<IDeckReader> DeckReaders
		{
			get { return PluginManager.Instance.GetPlugins<IDeckReader>(); }
		}

		public static IEnumerable<IDeckWriter> DeckWriters
		{
			get { return PluginManager.Instance.GetPlugins<IDeckWriter>(); }
		}

		public static IDataParse DataParse
		{
			get { return PluginManager.Instance.GetPlugin<IDataParse>(); }
		}

		public static bool ComponentsAvailable
		{
			get
			{
				return
					DbReader != null && DbWriter != null && Compressor != null && ImageParse != null && DeckReaders.Any() &&
					DeckWriters.Any() && DataParse != null;
			}
		}

		public static bool SetLanguage(Language language)
		{
			if (DbReader == null || DbWriter == null)
			{
				return false;
			}
			DbReader.Language = language;
			DbWriter.Language = language;
			return true;
		}
	}
}