using System.IO;
using System.IO.Compression;

namespace HyperPlugin
{
	public class GZipCompressor : ICompressor
	{
		#region ICompressor Members

		public string CompressorType
		{
			get { return "GZip"; }
		}

		public string Description
		{
			get { return "Compress/Decompress bytes"; }
		}

		public string Name
		{
			get { return "GZipCompressor"; }
		}

		public byte[] Compress(byte[] data)
		{
			var ms = new MemoryStream();
			var zipStream = new GZipStream(ms, CompressionMode.Compress);
			zipStream.Write(data, 0, data.Length);
			zipStream.Close();
			return ms.ToArray();
		}

		public byte[] Decompress(byte[] data, int length)
		{
			var srcMs = new MemoryStream(data);
			var zipStream = new GZipStream(srcMs, CompressionMode.Decompress);
			var ms = new MemoryStream();
			var bytes = new byte[length];
			int n;
			while ((n = zipStream.Read(bytes, 0, bytes.Length)) > 0)
			{
				ms.Write(bytes, 0, n);
			}
			zipStream.Close();
			return ms.ToArray();
		}

		public byte[] Decompress(byte[] data)
		{
			return Decompress(data, 40960);
		}

		#endregion
	}
}