using System.Collections.Generic;
using System.ServiceModel;
using HyperService.Common;

namespace HyperService.Game
{
	public interface IGameCallback
	{
		/// <summary>
		/// Refresh player list
		/// </summary>
		/// <param name="players"></param>
		[OperationContract(IsOneWay = true)]
		void RefreshPlayers(List<Player> players);

		/// <summary>
		/// Recieve message
		/// </summary>
		/// <param name="msg"></param>
		[OperationContract(IsOneWay = true)]
		void Receive(Message msg);

		/// <summary>
		/// Called when a player receives warning on connecting
		/// </summary>
		/// <param name="error"></param>
		[OperationContract(IsOneWay = true)]
		void ConnectionWarn(ConnectionResult error);

		/// <summary>
		/// Called when a player joined
		/// </summary>
		/// <param name="player"></param>
		[OperationContract(IsOneWay = true)]
		void PlayerJoin(Player player);

		/// <summary>
		/// Called when a player left
		/// </summary>
		/// <param name="player"></param>
		[OperationContract(IsOneWay = true)]
		void PlayerLeave(Player player);

		/// <summary>
		/// Called when game is opend
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="teams"></param>
		[OperationContract(IsOneWay = true)]
		void GameOpend(GameMode mode, List<Team> teams);

		/// <summary>
		/// Refresh teams info
		/// </summary>
		/// <param name="teams"></param>
		[OperationContract(IsOneWay = true)]
		void RefreshTeam(List<Team> teams);

		/// <summary>
		/// Called when a user chooses team
		/// </summary>
		/// <param name="player"></param>
		[OperationContract(IsOneWay = true)]
		void PlayerChoseTeam(Player player);

		/// <summary>
		/// Called when a user is ready
		/// </summary>
		/// <param name="player"></param>
		[OperationContract(IsOneWay = true)]
		void PlayerReady(Player player);

		/// <summary>
		/// Called when a user modifies
		/// </summary>
		[OperationContract(IsOneWay = true)]
		void GameStarted();

		/// <summary>
		/// Called when a user hanles cards
		/// </summary>
		/// <param name="player"></param>
		/// <param name="cards"></param>
		[OperationContract(IsOneWay = true)]
		void PlayerHandle(Player player, List<GameCard> cards);

		/// <summary>
		/// Player modifies life
		/// </summary>
		/// <param name="player"></param>
		/// <param name="mod"></param>
		[OperationContract(IsOneWay = true)]
		void PlayerModifyLife(Player player, int mod);

		/// <summary>
		/// Called when a player modifies poison counter
		/// </summary>
		/// <param name="player"></param>
		/// <param name="mod"></param>
		[OperationContract(IsOneWay = true)]
		void PlayerModifyPoison(Player player, int mod);

		/// <summary>
		/// Called when a player modifies step state
		/// </summary>
		/// <param name="player"></param>
		/// <param name="step"></param>
		[OperationContract(IsOneWay = true)]
		void PlayerModifyStep(Player player, Step step);

		/// <summary>
		/// Callede when a player rolls a die
		/// </summary>
		/// <param name="player"></param>
		/// <param name="sides"></param>
		/// <param name="num"></param>
		void PlayerRollDie(Player player,int sides, int num);

	}
}