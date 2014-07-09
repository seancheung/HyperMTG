using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyperKore.Xception
{
	public class RequestXception : HyperXception
	{
		private int tryCount;
		private string url;
		private string p;
		private Exception ex;

		public RequestXception(int tryCount, string url, string p, Exception ex)
		{
			// TODO: Complete member initialization
			this.tryCount = tryCount;
			this.url = url;
			this.p = p;
			this.ex = ex;
		}
	}
}
