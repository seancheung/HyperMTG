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
		public HallService()
		{
			IP = "localhost";
			Port = 5953;
			ClientCallbacks = new Dictionary<Client, IHallCallback>();
			Rooms = new List<Room>();
		}

		private Dictionary<Client, IHallCallback> ClientCallbacks { get; set; }
		private List<Room> Rooms { get; set; }

		private List<Client> Players
		{
			get { return ClientCallbacks.Keys.ToList(); }
		}

		private List<IHallCallback> Callbacks
		{
			get { return ClientCallbacks.Values.ToList(); }
		}

		public void Connect(Client client)
		{
			if (Players.Any(c => c.ID == client.ID))
			{
				lock (SyncObj)
				{
					CurrentCallback.OnConnect(ConnectionResult.UserExist);
				}
			}
			else
			{
				ClientCallbacks.Add(client, CurrentCallback);
				lock (SyncObj)
				{
					CurrentCallback.OnConnect(ConnectionResult.Success);
					Callbacks.ForEach(b =>
					{
						b.OnRefreshPlayers(Players);
						b.OnRefreshRooms(Rooms);
						b.OnEnterHall(client.ID);
					});
				}
			}
		}

		public void Disconnect(Guid client)
		{
			Client player = Players.Find(c => c.ID == client);
			ClientCallbacks.Remove(player);
			lock (SyncObj)
			{
				Callbacks.ForEach(b =>
				{
					b.OnRefreshPlayers(Players);
					b.OnRefreshRooms(Rooms);
					b.OnLeaveHall(client);
				});
			}
		}

		public void CreateRoom(Guid client, GameMode mode, GameFormat format, int size, string desc, string password)
		{
			Client player = Players.Find(c => c.ID == client);
			player.IsHost = true;
			var room = new Room {GameMode = mode, GameFormat = format, RoomSize = size, Desc = desc, Password = password};
			room.Players.Add(client);

			Rooms.Add(room);

			lock (SyncObj)
			{
				Callbacks.ForEach(b =>
				{
					b.OnRefreshPlayers(Players);
					b.OnRefreshRooms(Rooms);
				});
				CurrentCallback.OnCreateRoom(room.ID);
			}
		}

		public void JoinRoom(Guid client, Guid room, string password)
		{
			Client player = Players.Find(c => c.ID == client);
			Room rm = Rooms.Find(r => r.ID == room);
			if (rm.IsFull)
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
					Callbacks.ForEach(b =>
					{
						b.OnRefreshPlayers(Players);
						b.OnRefreshRooms(Rooms);
					});
					CurrentCallback.OnJoinRoom(JoinRoomResult.Success, room);
				}
			}
		}
	}
}