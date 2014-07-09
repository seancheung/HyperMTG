using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyperKore.Xception
{
	public class DownloadingXception : HyperXception
	{
		private string url;
		private string p;
		private Exception ex;
		private string path;

		public DownloadingXception(string url, string p, Exception ex)
		{
			// TODO: Complete member initialization
			this.url = url;
			this.p = p;
			this.ex = ex;
		}

		public DownloadingXception(string url, string path, string p, Exception ex)
		{
			// TODO: Complete member initialization
			this.url = url;
			this.path = path;
			this.p = p;
			this.ex = ex;
		}
	}
}
