using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyperKore.Xception
{
	public class IOXception : HyperXception
	{
		private string p;
		private Exception ex;

		public IOXception(string p, Exception ex)
		{
			// TODO: Complete member initialization
			this.p = p;
			this.ex = ex;
		}
	}
}
