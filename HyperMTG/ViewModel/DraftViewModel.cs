using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using HyperKore.Common;
using HyperKore.Utilities;
using HyperMTG.DraftSVC;
using HyperMTG.Helper;
using HyperMTG.Model;
using HyperMTG.Properties;
using HyperPlugin;
using HyperService.Common;
using HyperService.Draft;

namespace HyperMTG.ViewModel
{
	public class DraftViewModel : ObservableObject, IDraftCallback
	{
		private readonly ICompressor _compressor;
		private readonly IDBReader _dbReader;
		private readonly IDBWriter _dbWriter;
		private readonly Dispatcher _dispatcher;
		private readonly IImageParse _imageParse;
		private readonly DispatcherTimer _timer;
		private readonly int limitTime = 30;
		private readonly DraftService service;
		private ObservableCollection<ExCard> _currentBooster;
		private List<ObservableCollection<ExCard>> _currentBoosters;
		private bool _goRight;
		private ObservableCollection<ExCard> _hand;
		private string _ip;
		private bool _isConnected;
		private bool _isFree;
		private bool _isHosted;
		private bool _isStarted;
		private Client _localClient;
		private string _message;
		private string _messages;
		private ObservableCollection<Client> _onlineClients;
		private int _playerAmount;
		private double _ratio;
		private List<Set> _setSource;
		private Set[] _sets;
		private PageSize _size;
		private int _timerTick;
		private string info;
		private bool isWait;

		private DraftClient proxy;

		public DraftViewModel()
		{
			_dbReader = PluginManager.Instance.GetPlugin<IDBReader>();
			_compressor = PluginManager.Instance.GetPlugin<ICompressor>();
			_dbWriter = PluginManager.Instance.GetPlugin<IDBWriter>();
			_imageParse = PluginManager.Instance.GetPlugin<IImageParse>();
			_dispatcher = Application.Current.Dispatcher;
			if (_dbWriter != null && _dbReader != null)
			{
				_dbWriter.Language = Settings.Default.Language;
				_dbReader.Language = Settings.Default.Language;
				SetSource = _dbReader.LoadSets().Where(s => s.Local).ToList();
			}
			_timer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 1)};
			_timer.Tick += delegate { TimerTick++; };
			Hand = new ObservableCollection<ExCard>();
			CurrentBooster = new ObservableCollection<ExCard>();
			OnlineClients = new ObservableCollection<Client>();
			Sets = new Set[3];
			Size = new PageSize();
			Ratio = 0.5;
			_playerAmount = 2;
			IP = "localhost";
			MessageContent = string.Empty;
			service = new DraftService();
			LocalClient = new Client("Guest");
			Application.Current.Exit += delegate
			{
				if (IsConnected || IsHosted)
				{
					CloseExecute();
				}
			};

			Instance = this;
		}

		#region Prop

		public static DraftViewModel Instance { get; private set; }

		/// <summary>
		/// Whether game is started
		/// </summary>
		public bool IsStarted
		{
			get { return _isStarted; }
			set
			{
				_isStarted = value;
				RaisePropertyChanged("IsStarted");
			}
		}

		/// <summary>
		/// tick per second
		/// </summary>
		public int TimerTick
		{
			get { return _timerTick; }
			set
			{
				_timerTick = value;
				RaisePropertyChanged("TimerTick");
				CheckTime();
			}
		}

		/// <summary>
		/// Current cards to choose from
		/// </summary>
		public ObservableCollection<ExCard> CurrentBooster
		{
			get { return _currentBooster; }
			set
			{
				_currentBooster = value;
				RaisePropertyChanged("CurrentBooster");
			}
		}

		/// <summary>
		/// Hand card pile
		/// </summary>
		public ObservableCollection<ExCard> Hand
		{
			get { return _hand; }
			set
			{
				_hand = value;
				RaisePropertyChanged("Hand");
			}
		}

		/// <summary>
		/// Set selection slots
		/// </summary>
		public Set[] Sets
		{
			get { return _sets; }
			set
			{
				_sets = value;
				RaisePropertyChanged("Sets");
			}
		}

		/// <summary>
		/// Available sets
		/// </summary>
		public List<Set> SetSource
		{
			get { return _setSource; }
			set
			{
				_setSource = value;
				RaisePropertyChanged("SetSource");
			}
		}

		/// <summary>
		/// Player amount to start a draft game(when hosting)
		/// </summary>
		public int PlayerAmount
		{
			get { return _playerAmount; }
			set
			{
				if (_playerAmount != value)
				{
					_playerAmount = value;
					RaisePropertyChanged("PlayerAmount");
					if (IsConnected)
					{
						proxy.SetMaxPlayersAsync(value);
					}
				}
			}
		}

		/// <summary>
		/// Size of cards
		/// </summary>
		public PageSize Size
		{
			get { return _size; }
			set
			{
				_size = value;
				RaisePropertyChanged("Size");
			}
		}

		/// <summary>
		/// Zoom ratio of cards
		/// </summary>
		public double Ratio
		{
			get { return _ratio; }
			set
			{
				_ratio = value;
				RaisePropertyChanged("Ratio");
				Size.SetRatio(value);
			}
		}

		/// <summary>
		/// Server ip to host/connect to
		/// </summary>
		public string IP
		{
			get { return _ip; }
			set
			{
				_ip = value;
				RaisePropertyChanged("IP");
			}
		}

		/// <summary>
		/// Clients online
		/// </summary>
		public ObservableCollection<Client> OnlineClients
		{
			get { return _onlineClients; }
			set
			{
				_onlineClients = value;
				RaisePropertyChanged("OnlineClients");
			}
		}

		/// <summary>
		/// Chatting messages
		/// </summary>
		public string MessageContent
		{
			get { return _messages; }
			set
			{
				_messages = value;
				RaisePropertyChanged("MessageContent");
			}
		}

		/// <summary>
		/// Chatting message to send
		/// </summary>
		public string Message
		{
			get { return _message; }
			set
			{
				_message = value;
				RaisePropertyChanged("Message");
			}
		}

		/// <summary>
		/// Local player
		/// </summary>
		public Client LocalClient
		{
			get { return _localClient; }
			set
			{
				_localClient = value;
				RaisePropertyChanged("LocalClient");
			}
		}

		/// <summary>
		/// Whether local player is connected to a remote server
		/// </summary>
		public bool IsConnected
		{
			get { return _isConnected; }
			set
			{
				_isConnected = value;
				RaisePropertyChanged("IsConnected");
				RaisePropertyChanged("IsOnline");
			}
		}

		/// <summary>
		/// Whether service is hosted
		/// </summary>
		public bool IsHosted
		{
			get { return _isHosted; }
			set
			{
				_isHosted = value;
				RaisePropertyChanged("IsHosted");
				RaisePropertyChanged("IsOnline");
			}
		}

		/// <summary>
		/// Whether player is onlie
		/// </summary>
		public bool IsOnline
		{
			get { return IsHosted || IsConnected; }
		}

		/// <summary>
		/// Info message
		/// </summary>
		public string Info
		{
			get { return info; }
			set
			{
				info = value;
				RaisePropertyChanged("Info");
			}
		}

		#endregion

		#region Command

		/// <summary>
		/// Send chat message
		/// </summary>
		public ICommand SendMsgCommand
		{
			get { return new RelayCommand(SendMsgExecute, CanExecuteSendMsg); }
		}

		/// <summary>
		/// Join draft room
		/// </summary>
		public ICommand JoinCommand
		{
			get { return new RelayCommand(JoinExecute, CanExecuteJoin); }
		}

		/// <summary>
		/// Close connection
		/// </summary>
		public ICommand CloseCommand
		{
			get { return new RelayCommand(CloseExecute, CanExecuteClose); }
		}

		/// <summary>
		/// Host draft room
		/// </summary>
		public ICommand HostCommand
		{
			get { return new RelayCommand(HostExecute, CanExecuteHost); }
		}

		/// <summary>
		/// Pick card
		/// </summary>
		public ICommand PickCardCommand
		{
			get { return new RelayCommand<ExCard>(PickCardExecute, CanExecutePick); }
		}

		/// <summary>
		/// Start draft
		/// </summary>
		public ICommand StartCommand
		{
			get { return new RelayCommand(StartExecute, CanExecuteStart); }
		}

		/// <summary>
		/// Sync to deckbuilder
		/// </summary>
		public ICommand SyncCommand
		{
			get { return new RelayCommand(SyncExecute, CanExecuteSync); }
		}

		#endregion

		#region Execute

		public void JoinExecute()
		{
			InstanceContext context = new InstanceContext(this);
			proxy = new DraftClient(context);
			string servicePath = proxy.Endpoint.ListenUri.AbsolutePath;
			string serviceListenPort = proxy.Endpoint.Address.Uri.Port.ToString();
			proxy.Endpoint.Address =
				new EndpointAddress(string.Format("net.tcp://{0}:{1}{2}", IP, serviceListenPort, servicePath));

			try
			{
				proxy.Open();
				proxy.ConnectAsync(LocalClient);
			}
			catch (EndpointNotFoundException)
			{
				Info = "Connection Timeout";
			}
		}

		public void HostExecute()
		{
			IsHosted = service.StartService();
			if (IsHosted)
			{
				service.SetMaxPlayers(PlayerAmount);
				Info = "Service Hosted";
			}
		}

		public void SendMsgExecute()
		{
			if (string.IsNullOrWhiteSpace(Message))
			{
				return;
			}

			Message msg = new Message
			{
				Sender = LocalClient.ID,
				Content = Message,
				Time = DateTime.Now
			};

			proxy.SendMsg(msg);
			Message = string.Empty;
		}

		public void StartExecute()
		{
			proxy.StartDraft(Sets.Select(s => s.SetCode).ToList());
		}

		public void PickCardExecute(ExCard exCard)
		{
			Hand.Add(exCard);
			CurrentBooster.Remove(exCard);
			proxy.SwitchPack(LocalClient, CurrentBooster.Select(c => c.Card.ID).ToList());
		}

		public void CloseExecute()
		{
			if (IsConnected)
			{
				proxy.DisconnectAsync(LocalClient);
				IsConnected = false;
			}
			if (IsHosted)
			{
				service.StopService();
				IsHosted = false;
			}
			if (IsStarted)
			{
				OnEndDraft();
				IsStarted = false;
			}
			OnlineClients.Clear();
			Info = "Disconnected";
		}

		public void SyncExecute()
		{
			DeckBuiderViewModel.Instance.Deck.MainBoard.Clear();
			DeckBuiderViewModel.Instance.Deck.SideBoard.Clear();

			foreach (Card card in Hand.Select(c => c.Card))
			{
				DeckBuiderViewModel.Instance.Deck.MainBoard.Add(card);
			}
		}

		#endregion

		#region CanExecute

		public bool CanExecuteSendMsg()
		{
			return IsConnected;
		}

		public bool CanExecuteJoin()
		{
			return !IsConnected && IP.IsLegalIPAddress();
		}

		public bool CanExecuteClose()
		{
			return IsOnline;
		}

		public bool CanExecuteHost()
		{
			return !IsOnline && IP.IsLegalIPAddress();
		}

		public bool CanExecutePick(ExCard exCard)
		{
			return CurrentBooster != null && CurrentBooster.Any() && !isWait;
		}

		public bool CanExecuteStart()
		{
			return Sets.All(p => p != null) && IsHosted && IsConnected & !IsStarted;
		}

		public bool CanExecuteSync()
		{
			return !IsStarted && Hand.Count >= 45 && DeckBuiderViewModel.Instance != null;
		}

		#endregion

		#region Internal

		private void CheckTime()
		{
			if (TimerTick >= limitTime)
			{
				if (CurrentBooster.Any() && CanExecutePick(CurrentBooster.First()))
				{
					PickCardExecute(CurrentBooster.First());
				}
			}
		}

		private void OnMessage(string name, DateTime time, string content)
		{
			MessageContent += string.Format("[b]{0}[/b]([i]{1}[/i]): {2}\r\n", name, time, content);
		}

		#endregion

		#region Implementation of IDraftCallback

		public void RefreshMaxPlayers(int count)
		{
			PlayerAmount = count;
		}

		public void RefreshClients(List<Client> clients)
		{
			OnlineClients.Clear();
			foreach (Client client in clients)
			{
				OnlineClients.Add(client);
			}
		}

		public void OnConnect(ConnectionResult result)
		{
			switch (result)
			{
				case ConnectionResult.Success:
					IsConnected = true;
					Message = string.Empty;
					Info = "Connected";
					break;
				case ConnectionResult.RoomFull:
					break;
				case ConnectionResult.AlreadyStarted:
					break;
				case ConnectionResult.UserExist:
					break;
				case ConnectionResult.Banned:
					break;
				case ConnectionResult.Unkown:
					break;
				default:
					throw new ArgumentOutOfRangeException("result");
			}
		}

		public void OnReceive(Message msg)
		{
			Client sender = OnlineClients.FirstOrDefault(c => c.ID == msg.Sender);
			if (sender != null)
			{
				OnMessage(sender.Name, msg.Time, msg.Content);
			}
		}

		public void OnJoin(Client client)
		{
			OnMessage("System", DateTime.Now, string.Format("{0} Entered", client.Name));
		}

		public void OnLeave(Client client)
		{
			OnMessage("System", DateTime.Now, string.Format("{0} Left", client.Name));
		}

		public void OnPick(Client client)
		{
			OnMessage("System", DateTime.Now, string.Format("{0} picked", client.Name));
		}

		public void OnWait()
		{
			_timer.Stop();
			isWait = true;
		}

		public void OnSwitchPack(List<string> cardIDs)
		{
			ObservableCollection<ExCard> booster = new ObservableCollection<ExCard>();

			Task task = new Task(() =>
			{
				IEnumerable<Card> db = _dbReader.LoadCards();
				IEnumerable<Card> cards = BoosterTool.GetCards(db, cardIDs);
				foreach (Card card in cards)
				{
					booster.Add(new ExCard(_compressor, _dbReader, card, _dbWriter, _imageParse));
				}
			});
			task.Start();
			task.ContinueWith(t =>
			{
				CurrentBooster = booster;
				TimerTick = 0;
				_timer.Start();
				isWait = false;
			});
		}

		public void OnOpenBooster(string setCode)
		{
			ObservableCollection<ExCard> booster = new ObservableCollection<ExCard>();

			Task task = new Task(() =>
			{
				IEnumerable<Card> db = _dbReader.LoadCards();

				IEnumerable<Card> cards = db.Where(c => c.SetCode == setCode);

				foreach (Card card in BoosterTool.Generate(cards))
				{
					booster.Add(new ExCard(_compressor, _dbReader, card, _dbWriter, _imageParse));
				}
			});
			task.Start();
			task.ContinueWith(t =>
			{
				CurrentBooster = booster;
				TimerTick = 0;
				_timer.Start();
				isWait = false;
			});
		}

		public void OnStartDraft()
		{
			CurrentBooster.Clear();
			Hand.Clear();
			IsStarted = true;
		}

		public void OnEndDraft()
		{
			CurrentBooster.Clear();
			TimerTick = 0;
			_timer.Stop();
			IsStarted = false;
		}

		#region Async

		public IAsyncResult BeginRefreshClients(List<Client> clients, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndRefreshClients(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginOnConnect(ConnectionResult result, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndOnConnect(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginOnJoin(Client client, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndOnJoin(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginOnReceive(Message msg, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndOnReceive(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginOnLeave(Client client, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndOnLeave(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginOnPick(Client client, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndOnPick(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginOnWait(AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndOnWait(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginOnSwitchPack(List<string> cardIDs, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndOnSwitchPack(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginOnOpenBooster(string setCode, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndOnOpenBooster(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginOnStartDraft(AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndOnStartDraft(IAsyncResult result)
		{
			throw new NotImplementedException();
		}


		public IAsyncResult BeginOnEndDraft(AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndOnEndDraft(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginRefreshMaxPlayers(int count, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndRefreshMaxPlayers(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		#endregion

		#endregion
	}
}