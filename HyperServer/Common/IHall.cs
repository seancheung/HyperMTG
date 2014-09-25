using System;
using System.ServiceModel;

namespace HyperServer.Common
{
	[ServiceContract(CallbackContract = typeof (IHallCallback), SessionMode = SessionMode.Required)]
	public interface IHall
	{
		[OperationContract(IsInitiating = true)]
		void Connect(Client client);

		[OperationContract(IsOneWay = true,
			IsTerminating = true)]
		void Disconnect(Guid client);

		[OperationContract(IsOneWay = true)]
		void CreateRoom(Guid client, GameMode mode, GameFormat format, int size, string desc, string password);

		[OperationContract(IsOneWay = true)]
		void JoinRoom(Guid client, Guid room, string password);
	}
}