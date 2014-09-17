using System;
using HyperService.Draft;
using HyperService.Game;

namespace HyperHost
{
	internal class Program
	{
		private static DraftService draftService;
		private static GameService gameService;

		private static void Main(string[] args)
		{
			Console.WriteLine("Choose a service:");
			Console.WriteLine("-1\tDraft");
			Console.WriteLine("-2\tGame");

			string input = Console.ReadLine();
			while (string.IsNullOrWhiteSpace(input) || (input != "1" && input != "2"))
			{
				Console.WriteLine("Incorrect command");
				input = Console.ReadLine();
			}

			switch (input)
			{
				case "1":
					draftService = new DraftService();
					break;
				case "2":
					gameService = new GameService();
					break;
			}

			Console.WriteLine("-host\tstart service");

			input = Console.ReadLine();

			while (string.IsNullOrWhiteSpace(input) || input != "host")
			{
				Console.WriteLine("Incorrect command");
				input = Console.ReadLine();
			}
			bool result = false;
			if (draftService != null)
			{
				result = draftService.StartService();
			}
			else if (gameService != null)
			{
				result = gameService.StartService();
			}

			if (result)
			{
				Console.WriteLine("Hosted");
				Console.WriteLine("-stop\tstop service and quit");
			}

			while (Console.ReadLine() != "stop")
			{
				Console.WriteLine("Incorrect command");
			}

			if (draftService != null)
			{
				draftService.StopService();
			}
			if (gameService != null)
			{
				gameService.StopService();
			}
		}
	}
}