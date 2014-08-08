using HyperPlugin;

namespace HyperMTG.ViewModel
{
	public class GallaryViewModel
	{
		private readonly ICompressor _compressor;

		private readonly IDBReader _dbReader;

		public GallaryViewModel()
		{
			_dbReader = PluginManager.Instance.GetPlugin<IDBReader>();
			_compressor = PluginManager.Instance.GetPlugin<ICompressor>();
		}

		public byte[] Image
		{
			get { return _dbReader != null && _compressor != null ? _dbReader.LoadFile("jou/en/1", _compressor) : null; }
		}
	}
}