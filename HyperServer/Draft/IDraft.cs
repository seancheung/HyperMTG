using System.Collections.Generic;
using System.ServiceModel;
using HyperServer.Common;

namespace HyperServer.Draft
{
	[ServiceContract(CallbackContract = typeof (IDraftCallback), SessionMode = SessionMode.Required)]
	public interface IDraft
	{
		[OperationContract(IsInitiating = true)]
		void Connect(DraftPlayer client);

		[OperationContract(IsOneWay = true,
			IsTerminating = true)]
		void Disconnect(DraftPlayer client);

		[OperationContract(IsOneWay = true)]
		void HostGame(int maxPlayers, int timeLimit, List<string> setCodes);

		[OperationContract(IsOneWay = true)]
		void Start();

		[OperationContract(IsOneWay = true)]
		void SendMessage(Message msg);

		[OperationContract(IsOneWay = true)]
		void Ready(DraftPlayer player);

		[OperationContract(IsOneWay = true)]
		void SendPack(DraftPlayer player);

		[OperationContract(IsOneWay = true)]
		void End();
	}
}