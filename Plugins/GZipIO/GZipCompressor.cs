using HyperKore.IO;
using System.IO;
using System.IO.Compression;

namespace GZipIO
{
	public class GZipCompressor : ICompressor
	{
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
			MemoryStream ms = new MemoryStream();
			GZipStream zipStream = new GZipStream(ms, CompressionMode.Compress);
			zipStream.Write(data, 0, data.Length);
			zipStream.Close();
			return ms.ToArray();
		}

		public byte[] Decompress(byte[] data, int length)
		{
			MemoryStream srcMs = new MemoryStream(data);
			GZipStream zipStream = new GZipStream(srcMs, CompressionMode.Decompress);
			MemoryStream ms = new MemoryStream();
			byte[] bytes = new byte[length];
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
	}
}