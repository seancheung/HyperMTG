using System.Collections.Generic;
using System.ServiceModel;

namespace HyperService.Draft
{
	public interface IDraftCallBack
	{
		/// <summary>
		/// Refresh client list
		/// </summary>
		/// <param name="clients"></param>
		[OperationContract(IsOneWay = true)]
		void RefreshClients(IEnumerable<Client> clients);

		/// <summary>
		/// Recieve message
		/// </summary>
		/// <param name="msg"></param>
		[OperationContract(IsOneWay = true)]
		void Receive(Message msg);

		/// <summary>
		/// Called when a usered joined
		/// </summary>
		/// <param name="client"></param>
		[OperationContract(IsOneWay = true)]
		void UserJoin(Client client);

		/// <summary>
		/// Called when a usered left
		/// </summary>
		/// <param name="client"></param>
		[OperationContract(IsOneWay = true)]
		void UserLeave(Client client);

		/// <summary>
		/// Called when a user picked card
		/// </summary>
		/// <param name="client"></param>
		[OperationContract]
		void UserPick(Client client);

		/// <summary>
		/// Wait for server
		/// </summary>
		[OperationContract]
		void UserWait();

		/// <summary>
		/// User got switched pack
		/// </summary>
		/// <param name="cardIDs"></param>
		[OperationContract]
		void UserSwitchPack(IList<string> cardIDs);

		/// <summary>
		/// Open booster pack of specified set
		/// </summary>
		/// <param name="setCode"></param>
		[OperationContract]
		void UserOpenBooster(string setCode);

		/// <summary>
		/// Start draft
		/// </summary>
		[OperationContract]
		void UserStartDraft();

		/// <summary>
		/// End draft
		/// </summary>
		[OperationContract(IsOneWay = true)]
		void UserEndDraft();
	}
}