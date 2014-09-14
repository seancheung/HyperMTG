using System;
using System.Collections;
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
using HyperService.Draft;

namespace HyperMTG.ViewModel
{
	public class DraftViewModel : ObservableObject, IDraftCallback
	{
		private readonly List<ObservableCollection<ExCard>> _boosters;
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
		private string _messages;
		private ObservableCollection<Client> _onlineClients;
		private Set[] _packs;
		private int _playerAmount;
		private double _ratio;
		private List<Set> _setSource;
		private PageSize _size;
		private int _timerTick;

		private DraftClient proxy;
		private Client receiver;
		private string _message;
		private Client _localClient;
		private bool _isFree;
		private bool _isConnected;
		private bool _isHosted;
		private bool _isStarted;

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
			_boosters = new List<ObservableCollection<ExCard>>();
			Hand = new ObservableCollection<ExCard>();
			CurrentBooster = new ObservableCollection<ExCard>();
			OnlineClients = new ObservableCollection<Client>();
			Packs = new Set[3];
			Size = new PageSize();
			Ratio = 0.5;
			_playerAmount = 2;
			IP = "localhost:7997";
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

		public bool IsStarted
		{
			get { return _isStarted; }
			set
			{
				_isStarted = value;
				RaisePropertyChanged("IsStarted");
			}
		}

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

		public ObservableCollection<ExCard> CurrentBooster
		{
			get { return _currentBooster; }
			set
			{
				_currentBooster = value;
				RaisePropertyChanged("CurrentBooster");
			}
		}

		public ObservableCollection<ExCard> Hand
		{
			get { return _hand; }
			set
			{
				_hand = value;
				RaisePropertyChanged("Hand");
			}
		}

		public Set[] Packs
		{
			get { return _packs; }
			set
			{
				_packs = value;
				RaisePropertyChanged("Packs");
			}
		}

		public List<Set> SetSource
		{
			get { return _setSource; }
			set
			{
				_setSource = value;
				RaisePropertyChanged("SetSource");
			}
		}

		public int PlayerAmount
		{
			get { return _playerAmount; }
			set
			{
				_playerAmount = value;
				RaisePropertyChanged("PlayerAmount");
			}
		}

		public PageSize Size
		{
			get { return _size; }
			set
			{
				_size = value;
				RaisePropertyChanged("Size");
			}
		}

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

		public string IP
		{
			get { return _ip; }
			set
			{
				_ip = value;
				RaisePropertyChanged("IP");
			}
		}

		public ObservableCollection<Client> OnlineClients
		{
			get { return _onlineClients; }
			set
			{
				_onlineClients = value;
				RaisePropertyChanged("OnlineClients");
			}
		}

		public string MessageContent
		{
			get { return _messages; }
			set
			{
				_messages = value;
				RaisePropertyChanged("MessageContent");
			}
		}

		public string Message
		{
			get { return _message; }
			set
			{
				_message = value;
				RaisePropertyChanged("Message");
			}
		}

		public Client LocalClient
		{
			get { return _localClient; }
			set
			{
				_localClient = value;
				RaisePropertyChanged("LocalClient");
			}
		}

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

		public bool IsOnline
		{
			get { return IsHosted || IsConnected; }
		}

		#endregion

		#region Command

		public ICommand SendMsgCommand
		{
			get { return new RelayCommand(SendMsgExecute, CanExecuteSendMsg); }
		}

		public ICommand JoinCommand
		{
			get { return new RelayCommand(JoinExecute, CanExecuteJoin); }
		}

		public ICommand CloseCommand
		{
			get { return new RelayCommand(CloseExecute, CanExecuteClose); }
		}

		public ICommand HostCommand
		{
			get { return new RelayCommand(HostExecute, CanExecuteHost); }
		}

		public ICommand PickCardCommand
		{
			get { return new RelayCommand<ExCard>(PickCardExecute, CanExecutePick); }
		}

		public ICommand StartCommand
		{
			get { return new RelayCommand(StartExecute, CanExecuteStart); }
		}

		public ICommand SyncCommand
		{
			get { return new RelayCommand(SyncExecute, CanExecuteSync); }
		}

		#endregion

		#region Execute

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

			proxy.SayAsync(msg);
			Message = string.Empty;
		}

		public void JoinExecute()
		{
			var context = new InstanceContext(this);
			proxy = new DraftClient(context);
			string servicePath = proxy.Endpoint.ListenUri.AbsolutePath;
			string serviceListenPort = proxy.Endpoint.Address.Uri.Port.ToString();
			proxy.Endpoint.Address = new EndpointAddress("net.tcp://" + IP + servicePath);

			try
			{
				proxy.Open();
				proxy.ConnectAsync(LocalClient);
				IsConnected = true;
				Message = string.Empty;
			}
			catch
			{
				OnMessage("System", DateTime.Now, "Connection Timeout");
			}
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
				service.Stop();
				IsHosted = false;
			}
			OnlineClients.Clear();
		}

		public void HostExecute()
		{
			string[] ip = IP.Split(':');
			IsHosted = service.Start(ip[0], Int32.Parse(ip[1]));
		}

		public void PickCardExecute(ExCard exCard)
		{
			if (_currentBoosters == null)
			{
				_currentBoosters = _boosters.Take(_playerAmount).ToList();
				_boosters.RemoveRange(0, _playerAmount);
				_goRight = !_goRight;
			}

			var ran = new Random();

			foreach (var pack in _currentBoosters)
			{
				if (pack != CurrentBooster)
				{
					pack.RemoveAt(ran.Next(0, pack.Count));
				}
			}

			Hand.Add(exCard);
			CurrentBooster.Remove(exCard);

			if (_goRight)
			{
				ShiftRight(_currentBoosters);
			}
			else
			{
				ShiftLeft(_currentBoosters);
			}

			if (!_currentBoosters.All(b => b.Any()) && _boosters.Any())
			{
				_currentBoosters = _boosters.Take(_playerAmount).ToList();
				_boosters.RemoveRange(0, _playerAmount);
				_goRight = !_goRight;
			}

			CurrentBooster = _currentBoosters.First();
			TimerTick = 0;
			if (Hand.Count >= 45)
			{
				_currentBoosters = null;
				_timer.Stop();
			}
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

		public void StartExecute()
		{
			IEnumerable<Card> db = _dbReader.LoadCards();
			_boosters.Clear();
			CurrentBooster.Clear();
			Hand.Clear();
			IsStarted = true;

			var task = new Task(() =>
			{
				foreach (Set pack in Packs)
				{
					for (int i = 0; i < PlayerAmount; i++)
					{
						var result = new List<Card>();
						IEnumerable<Card> cards = db.Where(c => c.SetCode == pack.SetCode);

						result.AddRange(cards.Where(c => c.GetRarity() == Rarity.Mythic || c.GetRarity() == Rarity.Rare)
							.ToArray()
							.GetRandoms());
						result.AddRange(cards.Where(c => c.GetRarity() == Rarity.Uncommon)
							.ToArray()
							.GetRandoms(3));
						if (cards.Any(c => c.IsBasicLand()))
						{
							result.AddRange(cards.Where(c => c.GetRarity() == Rarity.Common)
								.ToArray()
								.GetRandoms(10));
							result.AddRange(cards.Where(c => c.IsBasicLand())
								.ToArray()
								.GetRandoms());
						}
						else
						{
							result.AddRange(cards.Where(c => c.GetRarity() == Rarity.Common)
								.ToArray()
								.GetRandoms(11));
						}

						var booster = new ObservableCollection<ExCard>();
						foreach (Card card in result)
						{
							booster.Add(new ExCard(_compressor, _dbReader, card, _dbWriter, _imageParse));
						}
						_dispatcher.BeginInvoke(new Action(() => _boosters.Add(booster)));
					}
				}
			});
			task.Start();
			task.ContinueWith(t =>
			{
				CurrentBooster = _boosters.FirstOrDefault();
				TimerTick = 0;
				if (Hand.Count >= 45) //finished
				{
					IsStarted = false;
					_timer.Stop();
				}
				else
				{
					_timer.Start();
				}
			});
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
			return IsConnected || IsHosted;
		}

		public bool CanExecuteHost()
		{
			return !IsHosted && !IsConnected && IP.IsLegalIPAddress();
		}

		public bool CanExecutePick(ExCard exCard)
		{
			return CurrentBooster != null;
		}

		public bool CanExecuteStart()
		{
			return _dbReader != null && _dbWriter != null && Packs != null && Packs.All(p => p != null);
		}

		public bool CanExecuteSync()
		{
			return Hand.Count >= 45 && DeckBuiderViewModel.Instance != null;
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

		private void ShiftRight(IList list, int k = 1)
		{
			if (list == null)
			{
				return;
			}
			int size = list.Count - 1;
			Reverse(list, 0, size - k);
			Reverse(list, size - k + 1, size);
			Reverse(list, 0, size);
		}

		private void ShiftLeft(IList list, int k = 1)
		{
			if (list == null)
			{
				return;
			}
			int size = list.Count - 1;
			Reverse(list, 0, k);
			Reverse(list, k + 1, size);
			Reverse(list, 0, size);
		}

		private void Reverse(IList list, int left, int right)
		{
			while (left <= right)
			{
				object l = list[left];
				object r = list[right];
				list.Insert(left, r);
				list.RemoveAt(left + 1);
				list.Insert(right, l);
				list.RemoveAt(right + 1);

				left++;
				right--;
			}
		}

		private void OnMessage(string name, DateTime time, string content)
		{
			MessageContent += string.Format("[b]{0}[/b]([i]{1}[/i]): {2}\r\n", name, time, content);
		}

		#endregion

		#region Implementation of IDraftCallback

		public void RefreshClients(List<Client> clients)
		{
			OnlineClients.Clear();
			foreach (Client client in clients)
			{
				OnlineClients.Add(client);
			}
		}

		public IAsyncResult BeginRefreshClients(List<Client> clients, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndRefreshClients(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public void Receive(Message msg)
		{
			Client sender = OnlineClients.FirstOrDefault(c => c.ID == msg.Sender);
			if (sender != null)
			{
				OnMessage(sender.Name,msg.Time,msg.Content);
			}
		}

		public IAsyncResult BeginReceive(Message msg, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndReceive(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public void UserJoin(Client client)
		{
			OnMessage("System", DateTime.Now, string.Format("{0} Entered", client.Name));
		}

		public IAsyncResult BeginUserJoin(Client client, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndUserJoin(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public void UserLeave(Client client)
		{
			OnMessage("System", DateTime.Now, string.Format("{0} Left", client.Name));
		}

		public IAsyncResult BeginUserLeave(Client client, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndUserLeave(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public void Picked(Client client, int index)
		{
			 
		}

		public IAsyncResult BeginPicked(Client client, int index, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndPicked(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}