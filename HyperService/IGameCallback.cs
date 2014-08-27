using System.ServiceModel;

namespace HyperService
{
	public interface IGameCallback
	{
		[OperationContract(IsOneWay = true)]
		void Receive(Player sender, string msg);

		[OperationContract(IsOneWay = true)]
		void ReceivePlay(Team team, Player player);

		[OperationContract(IsOneWay = true)]
		void UserEnter(Team team, Player player);

		[OperationContract(IsOneWay = true)]
		void UserLeave(Team team, Player player);

		[OperationContract(IsOneWay = true)]
		void Wait(Team team, Player player);
	}
}