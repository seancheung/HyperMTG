using System;
using System.Collections.Generic;
using System.ServiceModel;
using HyperService.Common;

namespace HyperService.Game
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
		ConcurrencyMode = ConcurrencyMode.Multiple,
		UseSynchronizationContext = false)]
	public class GameService : Service<GameService, IGame, IGameCallback>, IGame
	{
		private List<Player> _players;
		private List<Team> _teams;

		public GameService()
		{
			IP = "localhost";
			Port = 5943;
		}

		#region Implementation of IGame

		/// <summary>
		/// Connect to server
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public bool Connect(Player player)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Send message
		/// </summary>
		/// <param name="msg"></param>
		public void SendMsg(Message msg)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Open a game
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="playerAmount"></param>
		public void OpenGame(GameMode mode, int playerAmount)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Choose team
		/// </summary>
		/// <param name="player"></param>
		public void ChooseTeam(Player player)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Ready up
		/// </summary>
		/// <param name="player"></param>
		public void ReadyGame(Player player)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Start game
		/// </summary>
		public void StartGame()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Player handle cards
		/// </summary>
		/// <param name="player"></param>
		/// <param name="cards"></param>
		public void Handle(Player player, List<GameCard> cards)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Player modifies life
		/// </summary>
		/// <param name="player"></param>
		/// <param name="mod"></param>
		public void ModifyLife(Player player, int mod)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Player modifies poison counter
		/// </summary>
		/// <param name="player"></param>
		/// <param name="mod"></param>
		public void ModifyPoison(Player player, int mod)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Player modifies Step state
		/// </summary>
		/// <param name="player"></param>
		/// <param name="step"></param>
		public void ModifyStep(Player player, Step step)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Player rolls a die
		/// </summary>
		/// <param name="player"></param>
		/// <param name="sides"></param>
		public void RollDie(Player player, int sides)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// End game
		/// </summary>
		public void EndGame()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Disconnect from server
		/// </summary>
		/// <param name="player"></param>
		public void Disconnect(Player player)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}