using System.Collections.Generic;
using System.ServiceModel;
using HyperServer.Common;

namespace HyperServer.Draft
{
	public interface IDraftCallback
	{
		[OperationContract(IsOneWay = true)]
		void OnConnect(ConnectionResult result);

		[OperationContract(IsOneWay = true)]
		void RefreshGame(int maxPlayers, int timeLimit, List<string> setCodes);

		[OperationContract(IsOneWay = true)]
		void OnJoin(DraftPlayer client);

		[OperationContract(IsOneWay = true)]
		void OnLeave(DraftPlayer client);

		[OperationContract(IsOneWay = true)]
		void RefreshClients(List<DraftPlayer> clients);

		[OperationContract(IsOneWay = true)]
		void OnStart();

		[OperationContract(IsOneWay = true)]
		void OnMessage(Message msg);

		[OperationContract(IsOneWay = true)]
		void OnReady(DraftPlayer player);

		[OperationContract(IsOneWay = true)]
		void OnEnd();
	}
}