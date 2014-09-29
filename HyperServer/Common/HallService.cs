using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace HyperServer.Common
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
		ConcurrencyMode = ConcurrencyMode.Multiple,
		UseSynchronizationContext = false)]
	public class HallService : Service<IHall, IHallCallback>, IHall
	{
		private readonly List<Client> _clients = new List<Client>(); 
		private readonly Dictionary<Guid, IHallCallback> _clientCallbacks = new Dictionary<Guid, IHallCallback>();
		private readonly List<Room> _rooms = new List<Room>();

		public HallService()
		{
			IP = "localhost";
			Port = 5953;
		}

		private IEnumerable<IHallCallback> Callbacks
		{
			get { return _clientCallbacks.Values; }
		}

		public void Connect(Client client)
		{
			if (_clients.Any(c => c.ID == client.ID))
			{
				lock (SyncObj)
				{
					CurrentCallback.OnConnect(ConnectionResult.UserExist);
				}
			}
			else
			{
				_clients.Add(client);
				_clientCallbacks.Add(client.ID, CurrentCallback);
				lock (SyncObj)
				{
					CurrentCallback.OnConnect(ConnectionResult.Success);
					foreach (IHallCallback callback in Callbacks)
					{
						callback.OnRefreshPlayers(_clients);
						callback.OnRefreshRooms(_rooms);
						callback.OnEnterHall(client.ID);
					}
				}
			}
		}

		public void Disconnect(Guid client)
		{
			_clients.RemoveAll(c => c.ID == client);
			_clientCallbacks.Remove(client);
			lock (SyncObj)
			{
				foreach (IHallCallback callback in Callbacks)
				{
					callback.OnLeaveHall(client);
					callback.OnRefreshPlayers(_clients);
					callback.OnRefreshRooms(_rooms);
				}
			}
		}

		public void CreateRoom(Guid client, GameMode mode, GameFormat format, int size, string desc, string password)
		{
			Client player = _clients.First(c => c.ID == client);
			player.IsHost = true;
			var room = new Room {GameMode = mode, GameFormat = format, RoomSize = size, Desc = desc, Password = password};
			room.Players.Add(client);

			_rooms.Add(room);

			lock (SyncObj)
			{
				foreach (IHallCallback callback in Callbacks)
				{
					callback.OnRefreshPlayers(_clients);
					callback.OnRefreshRooms(_rooms);
				}
				CurrentCallback.OnCreateRoom(room.ID);
			}
		}

		public void JoinRoom(Guid client, Guid room, string password)
		{
			Room rm = _rooms.Find(r => r.ID == room);
			if (rm.Players.Count >= rm.RoomSize)
			{
				lock (SyncObj)
				{
					CurrentCallback.OnJoinRoom(JoinRoomResult.RoomFull, room);
				}
			}
			else if (rm.Password != password)
			{
				lock (SyncObj)
				{
					CurrentCallback.OnJoinRoom(JoinRoomResult.IncorrectPassword, room);
				}
			}
			else
			{
				rm.Players.Add(client);
				lock (SyncObj)
				{
					foreach (IHallCallback callback in Callbacks)
					{
						callback.OnRefreshPlayers(_clients);
						callback.OnRefreshRooms(_rooms);
					}
					CurrentCallback.OnJoinRoom(JoinRoomResult.Success, room);
				}
			}
		}
	}
}