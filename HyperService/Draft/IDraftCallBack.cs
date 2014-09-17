using System.Collections.Generic;
using System.ServiceModel;
using HyperService.Common;

namespace HyperService.Draft
{
	public interface IDraftCallBack
	{
		/// <summary>
		/// Refresh max players
		/// </summary>
		/// <param name="count"></param>
		[OperationContract(IsOneWay = true)]
		void RefreshMaxPlayers(int count);

		/// <summary>
		/// Refresh client list
		/// </summary>
		/// <param name="clients"></param>
		[OperationContract(IsOneWay = true)]
		void RefreshClients(List<Client> clients);

		/// <summary>
		/// Called when a client receives warning on connecting
		/// </summary>
		/// <param name="result"></param>
		[OperationContract(IsOneWay = true)]
		void OnConnect(ConnectionResult result);

		/// <summary>
		/// Recieve message
		/// </summary>
		/// <param name="msg"></param>
		[OperationContract(IsOneWay = true)]
		void OnReceive(Message msg);

		/// <summary>
		/// Called when a client joined
		/// </summary>
		/// <param name="client"></param>
		[OperationContract(IsOneWay = true)]
		void OnJoin(Client client);

		/// <summary>
		/// Called when a client left
		/// </summary>
		/// <param name="client"></param>
		[OperationContract(IsOneWay = true)]
		void OnLeave(Client client);

		/// <summary>
		/// Called when a client picked card
		/// </summary>
		/// <param name="client"></param>
		[OperationContract(IsOneWay = true)]
		void OnPick(Client client);

		/// <summary>
		/// Wait for server
		/// </summary>
		[OperationContract(IsOneWay = true)]
		void OnWait();

		/// <summary>
		/// Client got switched pack
		/// </summary>
		/// <param name="cardIDs"></param>
		[OperationContract(IsOneWay = true)]
		void OnSwitchPack(List<string> cardIDs);

		/// <summary>
		/// Open booster pack of specified set
		/// </summary>
		/// <param name="setCode"></param>
		[OperationContract(IsOneWay = true)]
		void OnOpenBooster(string setCode);

		/// <summary>
		/// Start draft
		/// </summary>
		[OperationContract(IsOneWay = true)]
		void OnStartDraft();

		/// <summary>
		/// End draft
		/// </summary>
		[OperationContract(IsOneWay = true)]
		void OnEndDraft();
	}
}