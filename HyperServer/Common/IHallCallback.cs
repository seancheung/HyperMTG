using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace HyperServer.Common
{
	public interface IHallCallback
	{
		[OperationContract(IsOneWay = true)]
		void OnConnect(ConnectionResult result);

		[OperationContract(IsOneWay = true)]
		void OnEnterHall(Guid client);

		[OperationContract(IsOneWay = true)]
		void OnLeaveHall(Guid client);

		[OperationContract(IsOneWay = true)]
		void OnRefreshRooms(List<Room> rooms);

		[OperationContract(IsOneWay = true)]
		void OnCreateRoom(Guid room);

		[OperationContract(IsOneWay = true)]
		void OnJoinRoom(JoinRoomResult result,Guid room);

		[OperationContract(IsOneWay = true)]
		void OnRefreshPlayers(List<Client> clients);
	}
}