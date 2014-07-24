using System.IO;
using System.Windows.Media.Imaging;

namespace HyperMTG.Helper
{
	public static class ImageHelper
	{
		public static BitmapImage ToBitmapImage(this byte[] byteArray)
		{
			BitmapImage bmp;

			try
			{
				bmp = new BitmapImage();
				bmp.BeginInit();
				bmp.StreamSource = new MemoryStream(byteArray);
				bmp.EndInit();
			}
			catch
			{
				bmp = null;
			}

			return bmp;
		}
	}
}