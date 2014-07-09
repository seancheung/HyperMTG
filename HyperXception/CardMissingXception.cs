using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyperKore.Xception
{
	public class CardMissingXception : HyperXception
	{
		private string p1;
		private string p2;
		private string p3;

		public CardMissingXception(string p1, string p2, string p3)
		{
			// TODO: Complete member initialization
			this.p1 = p1;
			this.p2 = p2;
			this.p3 = p3;
		}

		public CardMissingXception(string p1, string p2)
		{
			// TODO: Complete member initialization
			this.p1 = p1;
			this.p2 = p2;
		}
	}
}
