//http://www.cnblogs.com/over140/archive/0001/01/01/1524735.html

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HyperKore.Logger
{
	public sealed class Logger
	{
		private const string trace_exception = "\r\n***********************TRACE_EXCEPTION {0}***********************";
		private static DateTime CurrentLogFileDate = DateTime.Now;
		private static TextWriterTraceListener twtl;
		private const string log_root_directory = @"logs";
		private static string log_subdir;
		private const string FORMAT_TRACE_PARAM = "      {0} = {1}";

		public Logger()
		{

		}
	}
}
