using System.Collections.Generic;
using System.ServiceModel;
using HyperService.Common;

namespace HyperService.Draft
{
	[ServiceContract(CallbackContract = typeof (IDraftCallBack), SessionMode = SessionMode.Required)]
	public interface IDraft
	{
		/// <summary>
		/// Connect to server
		/// </summary>
		/// <param name="client"></param>
		/// <returns></returns>
		[OperationContract(IsInitiating = true)]
		void Connect(Client client);

		/// <summary>
		/// Set max player amount
		/// </summary>
		/// <param name="count"></param>
		[OperationContract(IsOneWay = true)]
		void SetMaxPlayers(int count);

		/// <summary>
		/// Send message
		/// </summary>
		/// <param name="msg"></param>
		[OperationContract(IsOneWay = true)]
		void SendMsg(Message msg);

		/// <summary>
		/// Start draft
		/// </summary>
		/// <param name="setCodes"></param>
		[OperationContract(IsOneWay = true)]
		void StartDraft(List<string> setCodes);

		/// <summary>
		/// Switch pack
		/// </summary>
		/// <param name="client"></param>
		/// <param name="cardIDs"></param>
		[OperationContract(IsOneWay = true)]
		void SwitchPack(Client client, List<string> cardIDs);

		/// <summary>
		/// End draft
		/// </summary>
		[OperationContract(IsOneWay = true)]
		void EndDraft();

		/// <summary>
		/// Disconnect from server
		/// </summary>
		/// <param name="client"></param>
		[OperationContract(IsOneWay = true,
			IsTerminating = true)]
		void Disconnect(Client client);
	}
}