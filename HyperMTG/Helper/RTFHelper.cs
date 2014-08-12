//http://www.cnblogs.com/whitewolf/archive/2011/01/09/1931290.html

using System;
using System.IO;
using System.Windows;
using System.Windows.Documents;

namespace HyperMTG.Helper
{
	public static class RTFHelper
	{
		public static string ToRTF(this FlowDocument document)
		{
			string rtf = string.Empty;
			var textRange = new TextRange(document.ContentStart, document.ContentEnd);
			using (var ms = new MemoryStream())
			{
				textRange.Save(ms, DataFormats.Rtf);
				ms.Seek(0, SeekOrigin.Begin);
				var sr = new StreamReader(ms);
				rtf = sr.ReadToEnd();
			}

			return rtf;
		}

		public static void LoadFromRTF(this FlowDocument document, string rtf)
		{
			if (string.IsNullOrEmpty(rtf))
			{
				throw new ArgumentNullException();
			}
			var textRange = new TextRange(document.ContentStart, document.ContentEnd);
			using (var ms = new MemoryStream())
			{
				using (var sw = new StreamWriter(ms))
				{
					sw.Write(rtf);
					sw.Flush();
					ms.Seek(0, SeekOrigin.Begin);
					textRange.Load(ms, DataFormats.Rtf);
				}
			}
		}
	}
}