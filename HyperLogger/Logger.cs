using System;
using System.Diagnostics;
using System.IO;

namespace HyperKore.Logger
{
	public sealed class Logger
	{
		static Logger()
		{
			Trace.AutoFlush = true;
			Trace.Listeners.Add(TraceListener);
		}

		private static string FileName
		{
			get { return DateTime.Now.ToFileTime().ToString(); }
		}

		private static string HeadLine
		{
			get { return string.Concat("*********************** ", DateTime.Now, " ***********************"); }
		}

		private static string LogDir
		{
			get
			{
				if (!Directory.Exists("logs"))
				{
					Directory.CreateDirectory("logs");
				}
				return "logs";
			}
		}

		private static string LogPath
		{
			get { return string.Concat(LogDir, "\\", FileName, ".log"); }
		}

		private static TextWriterTraceListener TraceListener
		{
			get { return new TextWriterTraceListener(LogPath); }
		}

		public static void Log(Exception ex, params object[] args)
		{
			new AsyncLogException(BeginTraceLog).BeginInvoke(ex, args, null, null);
		}

		private static void BeginTraceLog(Exception ex, params object[] args)
		{
			if (ex != null)
			{
				Trace.WriteLine(HeadLine);
			}
			while (ex != null)
			{
				Trace.WriteLine(string.Format("{0}\r\n{1}\r\n{2}\r\n{3}", ex.GetType(), ex.Message, ex.StackTrace, ex.Source));
				ex = ex.InnerException;
			}
			foreach (object arg in args)
			{
				Trace.WriteLine(string.Concat("ARG:\t", arg, "\r\n"));
			}
		}

		#region Nested type: AsyncLogException

		private delegate void AsyncLogException(Exception ex, params object[] args);

		#endregion
	}
}