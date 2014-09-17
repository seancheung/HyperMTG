using System.Collections.Generic;
using System.ServiceModel;
using HyperService.Common;

namespace HyperService.Game
{
	[ServiceContract(CallbackContract = typeof (IGameCallback), SessionMode = SessionMode.Required)]
	public interface IGame
	{
		/// <summary>
		/// Connect to server
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		[OperationContract(IsInitiating = true)]
		bool Connect(Player player);

		/// <summary>
		/// Send message
		/// </summary>
		/// <param name="msg"></param>
		[OperationContract(IsOneWay = true)]
		void SendMsg(Message msg);

		/// <summary>
		/// Open a game
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="playerAmount"></param>
		[OperationContract(IsOneWay = true)]
		void OpenGame(GameMode mode, int playerAmount);

		/// <summary>
		/// Choose team
		/// </summary>
		/// <param name="player"></param>
		[OperationContract(IsOneWay = true)]
		void ChooseTeam(Player player);

		/// <summary>
		/// Ready up
		/// </summary>
		/// <param name="player"></param>
		[OperationContract(IsOneWay = true)]
		void ReadyGame(Player player);

		/// <summary>
		/// Start game
		/// </summary>
		[OperationContract(IsOneWay = true)]
		void StartGame();

		/// <summary>
		/// Player handle cards
		/// </summary>
		/// <param name="player"></param>
		/// <param name="cards"></param>
		[OperationContract(IsOneWay = true)]
		void Handle(Player player, List<GameCard> cards);

		/// <summary>
		/// Player modifies life
		/// </summary>
		/// <param name="player"></param>
		/// <param name="mod"></param>
		[OperationContract(IsOneWay = true)]
		void ModifyLife(Player player, int mod);

		/// <summary>
		/// Player modifies poison counter
		/// </summary>
		/// <param name="player"></param>
		/// <param name="mod"></param>
		[OperationContract(IsOneWay = true)]
		void ModifyPoison(Player player, int mod);

		/// <summary>
		/// Player modifies Step state
		/// </summary>
		/// <param name="player"></param>
		/// <param name="step"></param>
		[OperationContract(IsOneWay = true)]
		void ModifyStep(Player player, Step step);

		/// <summary>
		/// Player rolls a die
		/// </summary>
		/// <param name="player"></param>
		/// <param name="sides"></param>
		[OperationContract(IsOneWay = true)]
		void RollDie(Player player, int sides);

		/// <summary>
		/// End game
		/// </summary>
		[OperationContract(IsOneWay = true)]
		void EndGame();

		/// <summary>
		/// Disconnect from server
		/// </summary>
		/// <param name="player"></param>
		[OperationContract(IsOneWay = true,
			IsTerminating = true)]
		void Disconnect(Player player);
	}
}