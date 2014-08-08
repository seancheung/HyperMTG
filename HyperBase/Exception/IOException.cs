using System;

namespace HyperKore.Exception
{
	public class IOException : HyperException
	{
		public IOException(System.Exception innerException)
		{
			throw new NotImplementedException();
		}
	}
}