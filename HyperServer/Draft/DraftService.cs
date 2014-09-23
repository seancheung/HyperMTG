using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using HyperServer.Common;

namespace HyperServer.Draft
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
		ConcurrencyMode = ConcurrencyMode.Multiple,
		UseSynchronizationContext = false)]
	public sealed class DraftService : Service<IDraft, IDraftCallback>, IDraft
	{
		private readonly Dictionary<DraftPlayer, IDraftCallback> _clientCallbacks =
			new Dictionary<DraftPlayer, IDraftCallback>();

		private bool _isStarted;
		private int _maxPlayers;
		private List<string> _setCodes;
		private int _timeLimit;
		private int _packNum;

		public DraftService()
		{
			IP = "localhost";
			Port = 5933;
			_maxPlayers = 1;
		}

		private List<DraftPlayer> Players
		{
			get { return _clientCallbacks.Keys.ToList(); }
		}

		private IEnumerable<IDraftCallback> Callbacks
		{
			get { return _clientCallbacks.Values.ToList(); }
		}

		#region Implementation of IDraft

		public void Disconnect(DraftPlayer client)
		{
			foreach (DraftPlayer key in Players.Where(c => c.ID == client.ID))
			{
				lock (SyncObj)
				{
					_clientCallbacks.Remove(key);
					foreach (IDraftCallback callback in Callbacks)
					{
						callback.OnLeave(client);
						callback.RefreshClients(Players);
					}
				}
			}
		}

		public void HostGame(int maxPlayers, int timeLimit, List<string> setCodes)
		{
			_maxPlayers = maxPlayers;
			_timeLimit = timeLimit;
			_setCodes = setCodes;
			CurrentCallback.RefreshGame(maxPlayers, timeLimit, setCodes);
		}

		public void Start()
		{
			_isStarted = true;
			_packNum = 0;
			lock (SyncObj)
			{
				foreach (DraftPlayer player in Players)
				{
					player.IsReady = false;
				}
				foreach (IDraftCallback callback in Callbacks)
				{
					callback.OnStart();
					callback.RefreshClients(Players);
					callback.OnOpenBooster(_setCodes[_packNum]);
				}
			}
		}

		public void SendMessage(Message msg)
		{
			lock (SyncObj)
			{
				foreach (IDraftCallback callback in Callbacks)
				{
					callback.OnMessage(msg);
				}
			}
		}

		public void Ready(DraftPlayer player)
		{
			lock (SyncObj)
			{
				foreach (DraftPlayer key in Players.Where(k => k.ID == player.ID))
				{
					key.IsReady = true;
				}
				foreach (IDraftCallback callback in Callbacks)
				{
					callback.OnReady(player);
					callback.RefreshClients(Players);
				}
			}
		}

		public void SendPack(DraftPlayer player)
		{
			lock (SyncObj)
			{
				foreach (DraftPlayer key in Players.Where(k => k.ID == player.ID))
				{
					key.HandList = player.HandList;
					key.PoolList = player.PoolList;
					key.IsReady = true;
				}

				if (Players.All(p=>p.IsReady))
				{
					if (Players.All(p=>p.PoolList.Any()))
					{
						if (Players.All(p=>p.HandList.Any()))
						{
							//switch
						}
						else
						{
							foreach (KeyValuePair<DraftPlayer, IDraftCallback> clientCallback in _clientCallbacks)
							{
								clientCallback.Value.OnReceivePack(clientCallback.Key.PoolList);
							}
						}
					}
					else
					{
						_packNum++;
						if (_packNum < _setCodes.Count)
						{
							foreach (IDraftCallback callback in Callbacks)
							{
								callback.OnOpenBooster(_setCodes[_packNum]);
							}
						}
						else
						{
							End();
						}
					}

					foreach (DraftPlayer draftPlayer in Players)
					{
						draftPlayer.IsReady = false;
					}
				}

				foreach (IDraftCallback callback in Callbacks)
				{
					callback.RefreshClients(Players);
				}
			}
		}

		public void End()
		{
			_isStarted = false;
			lock (SyncObj)
			{
				foreach (IDraftCallback callback in Callbacks)
				{
					callback.OnEnd();
				}
			}
		}

		public void Connect(DraftPlayer client)
		{
			if (Players.Count >= _maxPlayers)
			{
				lock (SyncObj)
				{
					CurrentCallback.OnConnect(ConnectionResult.RoomFull);
				}
			}
			else if (_isStarted)
			{
				lock (SyncObj)
				{
					CurrentCallback.OnConnect(ConnectionResult.AlreadyStarted);
				}
			}
			else if (_clientCallbacks.Any(c => c.Key == client))
			{
				lock (SyncObj)
				{
					CurrentCallback.OnConnect(ConnectionResult.UserExist);
				}
			}
			else
			{
				lock (SyncObj)
				{
					_clientCallbacks.Add(client, CurrentCallback);
					CurrentCallback.OnConnect(ConnectionResult.Success);
					if (!client.IsHost)
					{
						CurrentCallback.RefreshGame(_maxPlayers, _timeLimit, _setCodes);
					}
					foreach (IDraftCallback callback in Callbacks)
					{
						callback.OnJoin(client);
						callback.RefreshClients(Players);
					}
				}
			}
		}

		#endregion
	}
}