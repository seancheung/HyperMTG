using HyperKore.Plugin;
namespace HyperKore.IO
{
	public interface ICompressor : IPlugin
	{
		/// <summary>
		/// Type of the compressor
		/// </summary>
		string CompressorType { get; }

		/// <summary>
		/// Compress target byte array
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		byte[] Compress(byte[] data);

		/// <summary>
		/// Decompress target byte array with original length
		/// </summary>
		/// <param name="data"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		byte[] Decompress(byte[] data, int length);

		/// <summary>
		/// Decompress target byte array with default length of 40960
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		byte[] Decompress(byte[] data);
	}
}