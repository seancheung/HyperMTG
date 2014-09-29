using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows.Input;
using HyperMTGMain.HallSR;
using HyperMTGMain.Helper;
using HyperMTGMain.View;
using HyperServer.Common;
using IHallCallback = HyperMTGMain.HallSR.IHallCallback;

namespace HyperMTGMain.ViewModel
{
	public class OnlineViewModel : ObservableClass, IHallCallback
	{
		private static OnlineViewModel _instance;
		private Client _client;
		private List<Client> _clients;
		private HallClient _proxy;
		private List<Room> _rooms;

		private OnlineViewModel()
		{
		}

		public string IP { get; set; }

		public string UserName { get; set; }

		public Room Room { get; set; }

		public Client Client
		{
			get { return _client; }
			private set
			{
				_client = value;
				OnPropertyChanged("Client");
			}
		}

		public bool IsConnected { get; set; }

		public List<Client> Clients
		{
			get { return _clients; }
			private set
			{
				_clients = value;
				OnPropertyChanged("Clients");
			}
		}

		public List<Room> Rooms
		{
			get { return _rooms; }
			private set
			{
				_rooms = value;
				OnPropertyChanged("Rooms");
			}
		}

		public ICommand ConnectCommand
		{
			get { return new RelayCommand(Connect, CanConnect); }
		}

		public ICommand CreateRoomCommand
		{
			get { return new RelayCommand(CreateRoom, CanCreateRoom); }
		}

		public ICommand JoinRoomCommand
		{
			get { return new RelayCommand(JoinRoom, CanJoinRoom); }
		}

		internal static OnlineViewModel Instance
		{
			get { return _instance ?? (_instance = new OnlineViewModel()); }
		}

		#region Room params

		public Room SelectedRoom { get; set; }

		public GameMode GameMode { get; set; }

		public GameFormat GameFormat { get; set; }

		public int RoomSize { get; set; }

		public string Desc { get; set; }

		public string Password { get; set; }

		#endregion

		public void OnConnect(ConnectionResult result)
		{
			if (result == ConnectionResult.Success)
			{
				IsConnected = true;
				ViewManager.RoomListWindow.Show();
			}
			else
			{
				ViewModelManager.MessageViewModel.Message("Failed: {0}", result);
			}
		}

		public IAsyncResult BeginOnConnect(ConnectionResult result, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndOnConnect(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public void OnEnterHall(Guid client)
		{
		}

		public IAsyncResult BeginOnEnterHall(Guid client, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndOnEnterHall(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public void OnLeaveHall(Guid client)
		{
		}

		public IAsyncResult BeginOnLeaveHall(Guid client, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndOnLeaveHall(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public void OnRefreshRooms(List<Room> rooms)
		{
			Rooms = rooms;
		}

		public IAsyncResult BeginOnRefreshRooms(List<Room> rooms, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndOnRefreshRooms(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public void OnCreateRoom(Guid room)
		{
			Room = Rooms.Find(r => r.ID == Room.ID);
		}

		public IAsyncResult BeginOnCreateRoom(Guid room, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndOnCreateRoom(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public void OnJoinRoom(JoinRoomResult result, Guid room)
		{
			if (result == JoinRoomResult.Success)
			{
				Room = Rooms.Find(r => r.ID == Room.ID);
			}
			else
			{
				ViewModelManager.MessageViewModel.Message("Failed: {0}", result);
			}
		}

		public IAsyncResult BeginOnJoinRoom(JoinRoomResult result, Guid room, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndOnJoinRoom(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public void OnRefreshPlayers(List<Client> clients)
		{
			Clients = clients;
			Client = Clients.Find(c => c.ID == Client.ID);
		}

		public IAsyncResult BeginOnRefreshPlayers(List<Client> clients, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndOnRefreshPlayers(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		private void Connect()
		{
			if (_proxy != null)
			{
				_proxy.Close();
			}

			_proxy = new HallClient(new InstanceContext(this));
			string servicePath = _proxy.Endpoint.ListenUri.AbsolutePath;
			int serviceListenPort = _proxy.Endpoint.Address.Uri.Port;
			_proxy.Endpoint.Address =
				new EndpointAddress(string.Format("net.tcp://{0}:{1}{2}", IP, serviceListenPort, servicePath));
			Client = new Client(UserName);
			Room = null;

			try
			{
				_proxy.Open();
				_proxy.ConnectAsync(Client);
			}
			catch (EndpointNotFoundException)
			{
				ViewModelManager.MessageViewModel.Message("Connection Timeout");
			}
		}

		private bool CanConnect()
		{
			return !string.IsNullOrWhiteSpace(IP) && !string.IsNullOrWhiteSpace(UserName);
		}

		public void Close()
		{
			if (_proxy != null && _proxy.State == CommunicationState.Opened)
			{
				_proxy.DisconnectAsync(Client.ID);
				_proxy.Close();
			}

			_proxy = null;
			IsConnected = false;
		}

		private void CreateRoom()
		{
			_proxy.CreateRoom(Client.ID, GameMode, GameFormat, RoomSize, Desc, Password);
		}

		private bool CanCreateRoom()
		{
			return IsConnected && Room == null && RoomSize > 1;
		}

		private void JoinRoom()
		{
			_proxy.JoinRoom(Client.ID, SelectedRoom.ID, Password);
		}

		private bool CanJoinRoom()
		{
			return IsConnected && Room == null && SelectedRoom != null;
		}
	}
}