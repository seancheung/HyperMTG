using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using HyperService.Common;

namespace HyperService.Draft
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
		ConcurrencyMode = ConcurrencyMode.Multiple,
		UseSynchronizationContext = false)]
	public class DraftService : Service<DraftService, IDraft, IDraftCallBack>, IDraft
	{
		private readonly Dictionary<Client, IDraftCallBack> _clientCallbacks = new Dictionary<Client, IDraftCallBack>();
		private readonly List<Client> _clientList = new List<Client>();
		private Dictionary<int, List<string>> _cardList;
		private bool _isStarted;
		private int _maxPlayers;
		private List<string> _setCodes;
		private bool _shiftLeft;

		public DraftService()
		{
			IP = "localhost";
			Port = 5933;
		}

		#region Implementation of IDraft

		/// <summary>
		/// Connect to server
		/// </summary>
		/// <param name="client"></param>
		/// <returns></returns>
		public void Connect(Client client)
		{
			if (_isStarted)
			{
				lock (SyncObj)
				{
					CurrentCallback.OnConnect(ConnectionResult.AlreadyStarted);
				}
			}
			if (_clientList.Count >= _maxPlayers)
			{
				lock (SyncObj)
				{
					CurrentCallback.OnConnect(ConnectionResult.RoomFull);
				}
			}
			if (!_clientCallbacks.ContainsValue(CurrentCallback) && _clientCallbacks.All(c => c.Key.ID != client.ID))
			{
				lock (SyncObj)
				{
					_clientCallbacks.Add(client, CurrentCallback);
					_clientList.Add(client);

					foreach (Client key in _clientCallbacks.Keys)
					{
						IDraftCallBack callback = _clientCallbacks[key];
						try
						{
							callback.RefreshClients(_clientList);
							callback.OnJoin(client);
						}
						catch
						{
							_clientCallbacks.Remove(key);
							CurrentCallback.OnConnect(ConnectionResult.Unkown);
						}
					}
				}
				CurrentCallback.OnConnect(ConnectionResult.Success);
			}
			else
			{
				CurrentCallback.OnConnect(ConnectionResult.UserExist);
			}
		}

		/// <summary>
		/// Set max player amount
		/// </summary>
		/// <param name="count"></param>
		public void SetMaxPlayers(int count)
		{
			_maxPlayers = count;
			lock (SyncObj)
			{
				foreach (IDraftCallBack callback in _clientCallbacks.Values)
				{
					callback.RefreshMaxPlayers(count);
				}
			}
		}

		/// <summary>
		/// Send message
		/// </summary>
		/// <param name="msg"></param>
		public void SendMsg(Message msg)
		{
			lock (SyncObj)
			{
				foreach (IDraftCallBack callback in _clientCallbacks.Values)
				{
					callback.OnReceive(msg);
				}
			}
		}

		/// <summary>
		/// Switch pack
		/// </summary>
		/// <param name="client"></param>
		/// <param name="cardIDs"></param>
		public void SwitchPack(Client client, List<string> cardIDs)
		{
			lock (SyncObj)
			{
				Client clnt = _clientList.FirstOrDefault(c => c.ID == client.ID);
				if (clnt != null)
				{
					clnt.IsDone = true;
					_clientCallbacks[clnt].OnWait();
				}

				_cardList.Add(_clientList.FindIndex(c => c.ID == client.ID), cardIDs);

				foreach (IDraftCallBack callback in _clientCallbacks.Values)
				{
					callback.RefreshClients(_clientList);
					callback.OnPick(client);
				}

				if (_clientList.All(c => c.IsDone))
				{
					_clientList.ForEach(c => c.IsDone = false);
					foreach (IDraftCallBack callback in _clientCallbacks.Values)
					{
						callback.RefreshClients(_clientList);
					}

					if (_cardList.Values.All(s => s.Any())) //if there are cards left, continue switching
					{
						foreach (KeyValuePair<Client, IDraftCallBack> clientCallback in _clientCallbacks)
						{
							int index = _clientList.IndexOf(clientCallback.Key) + (_shiftLeft ? 1 : -1);

							if (index >= _clientList.Count)
							{
								index = 0;
							}
							else if (index < 0)
							{
								index = _clientList.Count - 1;
							}
							clientCallback.Value.OnSwitchPack(_cardList[index]);
						}
					}
					else
					{
						_setCodes.RemoveAt(0);
						if (_setCodes.Any()) //open new booster pack
						{
							_shiftLeft = !_shiftLeft;
							foreach (IDraftCallBack callback in _clientCallbacks.Values)
							{
								callback.OnOpenBooster(_setCodes.First());
							}
						}
						else //Draft end
						{
							foreach (IDraftCallBack callback in _clientCallbacks.Values)
							{
								callback.OnEndDraft();
							}
						}
					}

					_cardList.Clear();
				}
			}
		}

		/// <summary>
		/// Start draft
		/// </summary>
		/// <param name="setCodes"></param>
		public void StartDraft(List<string> setCodes)
		{
			_setCodes = setCodes;
			_isStarted = true;

			lock (SyncObj)
			{
				_cardList = new Dictionary<int, List<string>>();
				_shiftLeft = true;
				_clientList.ForEach(c => c.IsDone = false);
				foreach (IDraftCallBack callback in _clientCallbacks.Values)
				{
					callback.OnStartDraft();
					callback.RefreshClients(_clientList);
					callback.OnOpenBooster(setCodes.First());
				}
			}
		}

		/// <summary>
		/// End draft
		/// </summary>
		public void EndDraft()
		{
			lock (SyncObj)
			{
				foreach (IDraftCallBack callback in _clientCallbacks.Values)
				{
					callback.OnEndDraft();
				}
			}
			_isStarted = false;
		}

		/// <summary>
		/// Disconnect from server
		/// </summary>
		/// <param name="client"></param>
		public void Disconnect(Client client)
		{
			foreach (Client c in _clientCallbacks.Keys.Where(c => client.ID == c.ID))
			{
				lock (SyncObj)
				{
					_clientList.Remove(c);
					_clientCallbacks.Remove(c);
					foreach (IDraftCallBack callback in _clientCallbacks.Values)
					{
						callback.RefreshClients(_clientList);
						callback.OnLeave(client);
						callback.OnEndDraft();
					}
				}
				return;
			}
		}

		#endregion
	}
}