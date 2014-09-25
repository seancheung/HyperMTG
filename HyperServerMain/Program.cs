using System;
using HyperServer.Common;

namespace HyperServerMain
{
	internal class Program
	{
		private static HallService _service;

		private static void Main(string[] args)
		{
			ShowCommands();
			string input = Console.ReadLine();
			while (input != "-q")
			{
				switch (input)
				{
					case "-s":
						Start();
						break;
					case "-p":
						Stop();
						break;
					case "-h":
						ShowCommands();
						break;
					default:
						Warn();
						break;
				}

				input = Console.ReadLine();
			}
		}

		private static void Start()
		{
			Stop();
			_service = new HallService();
			if (_service.StartService())
			{
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("Server started!");
				Console.ResetColor();
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Failed to start server!");
				Console.ResetColor();
			}
		}

		private static void Stop()
		{
			if (_service != null)
			{
				if (_service.StopService())
				{
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("Server stopped!");
					Console.ResetColor();
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Failed to stop server!");
					Console.ResetColor();
				}
			}
		}

		private static void ShowCommands()
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("-s\tStart server");
			Console.WriteLine("-p\tStop server");
			Console.WriteLine("-q\tStop and quit");
			Console.WriteLine("-h\tHelp");
			Console.ResetColor();
		}

		private static void Warn()
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Error command! Enter '-h' for help");
			Console.ResetColor();
		}
	}
}