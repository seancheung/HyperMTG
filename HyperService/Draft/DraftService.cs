using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using HyperKore.Logger;

namespace HyperService.Draft
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
		ConcurrencyMode = ConcurrencyMode.Multiple,
		UseSynchronizationContext = false)]
	public class DraftService : IDraft
	{
		private readonly Dictionary<Client, IDraftCallBack> clientCallbacks = new Dictionary<Client, IDraftCallBack>();
		private readonly List<Client> clientList = new List<Client>();
		private readonly object syncObj = new object();
		private IList<string> _setCodes;
		private Dictionary<int, IList<string>> cardList;
		private ServiceHost host;
		private bool shiftLeft;
		private int turn;

		private IDraftCallBack _currentCallback
		{
			get { return OperationContext.Current.GetCallbackChannel<IDraftCallBack>(); }
		}

		/// <summary>
		/// UserStartDraft service
		/// </summary>
		/// <param name="ip"></param>
		/// <param name="port"></param>
		/// <returns></returns>
		public bool StartService(string ip, int port)
		{
			Uri tcpAdrs = new Uri(string.Format("net.tcp://{0}:{1}/HyperService.Draft/", ip, port));

			Uri httpAdrs = new Uri(string.Format("http://{0}:{1}/HyperService.Draft/", ip, port + 1));

			Uri[] baseAdresses = {tcpAdrs, httpAdrs};

			host = new ServiceHost(
				typeof (DraftService), baseAdresses);

			NetTcpBinding tcpBinding =
				new NetTcpBinding(SecurityMode.None, true);

			tcpBinding.MaxConnections = 100;

			ServiceThrottlingBehavior throttle;
			throttle =
				host.Description.Behaviors.Find<ServiceThrottlingBehavior>();
			if (throttle == null)
			{
				throttle = new ServiceThrottlingBehavior();
				throttle.MaxConcurrentCalls = 100;
				throttle.MaxConcurrentSessions = 100;
				host.Description.Behaviors.Add(throttle);
			}

			tcpBinding.ReceiveTimeout = new TimeSpan(20, 0, 0);
			tcpBinding.ReliableSession.Enabled = true;
			tcpBinding.ReliableSession.InactivityTimeout =
				new TimeSpan(20, 0, 10);

			host.AddServiceEndpoint(typeof (IDraft),
				tcpBinding, "tcp");

			ServiceMetadataBehavior mBehave =
				new ServiceMetadataBehavior();
			host.Description.Behaviors.Add(mBehave);

			host.AddServiceEndpoint(typeof (IMetadataExchange),
				MetadataExchangeBindings.CreateMexTcpBinding(),
				string.Format("net.tcp://{0}:{1}/HyperService.Draft/mex", ip, port - 1));

			try
			{
				host.Open();
				return true;
			}
			catch (Exception ex)
			{
				Logger.Log(ex, this);
				return false;
			}
		}

		/// <summary>
		/// Stop service
		/// </summary>
		/// <returns></returns>
		public bool StopService()
		{
			if (host != null)
			{
				try
				{
					host.Close();
					return true;
				}
				catch (Exception ex)
				{
					Logger.Log(ex, this);
					return false;
				}
			}
			return true;
		}

		#region Implementation of IDraft

		/// <summary>
		/// Connect to server
		/// </summary>
		/// <param name="client"></param>
		/// <returns></returns>
		public bool Connect(Client client)
		{
			if (!clientCallbacks.ContainsValue(_currentCallback) && clientCallbacks.All(c => c.Key.ID != client.ID))
			{
				lock (syncObj)
				{
					clientCallbacks.Add(client, _currentCallback);
					clientList.Add(client);

					foreach (Client key in clientCallbacks.Keys)
					{
						IDraftCallBack callback = clientCallbacks[key];
						try
						{
							callback.RefreshClients(clientList);
							callback.UserJoin(client);
						}
						catch
						{
							clientCallbacks.Remove(key);
							return false;
						}
					}
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Send message
		/// </summary>
		/// <param name="msg"></param>
		public void SendMsg(Message msg)
		{
			lock (syncObj)
			{
				foreach (IDraftCallBack callback in clientCallbacks.Values)
				{
					callback.Receive(msg);
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
			lock (syncObj)
			{
				Client clnt = clientList.FirstOrDefault(c => c.ID == client.ID);
				if (clnt != null)
				{
					clnt.IsDone = true;
					clientCallbacks[clnt].UserWait();
				}

				cardList.Add(clientList.FindIndex(c => c.ID == client.ID), cardIDs);

				foreach (IDraftCallBack callback in clientCallbacks.Values)
				{
					callback.RefreshClients(clientList);
					callback.UserPick(client);
				}

				if (clientList.All(c => c.IsDone))
				{
					clientList.ForEach(c => c.IsDone = false);
					foreach (IDraftCallBack callback in clientCallbacks.Values)
					{
						callback.RefreshClients(clientList);
					}

					if (cardList.Values.All(s => s.Any())) //if there are cards left, continue switching
					{
						foreach (KeyValuePair<Client, IDraftCallBack> clientCallback in clientCallbacks)
						{
							int index = clientList.IndexOf(clientCallback.Key) + (shiftLeft ? 1 : -1);

							if (index >= clientList.Count)
							{
								index = 0;
							}
							else if (index < 0)
							{
								index = clientList.Count - 1;
							}
							clientCallback.Value.UserSwitchPack(cardList[index]);
						}
					}
					else
					{
						_setCodes.RemoveAt(0);
						if (_setCodes.Any()) //open new booster pack
						{
							shiftLeft = !shiftLeft;
							foreach (IDraftCallBack callback in clientCallbacks.Values)
							{
								callback.UserOpenBooster(_setCodes.First());
							}
						}
						else //Draft end
						{
							foreach (IDraftCallBack callback in clientCallbacks.Values)
							{
								callback.UserEndDraft();
							}
						}
					}

					cardList.Clear();
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

			lock (syncObj)
			{
				cardList = new Dictionary<int, IList<string>>();
				shiftLeft = true;
				clientList.ForEach(c => c.IsDone = false);
				foreach (IDraftCallBack callback in clientCallbacks.Values)
				{
					callback.UserStartDraft();
					callback.RefreshClients(clientList);
					callback.UserOpenBooster(setCodes.First());
				}
			}
		}

		/// <summary>
		/// End draft
		/// </summary>
		public void EndDraft()
		{
			lock (syncObj)
			{
				foreach (IDraftCallBack callback in clientCallbacks.Values)
				{
					callback.UserEndDraft();
				}
			}
		}

		/// <summary>
		/// Disconnect from server
		/// </summary>
		/// <param name="client"></param>
		public void Disconnect(Client client)
		{
			foreach (Client c in clientCallbacks.Keys.Where(c => client.ID == c.ID))
			{
				lock (syncObj)
				{
					clientList.Remove(c);
					clientCallbacks.Remove(c);
					foreach (IDraftCallBack callback in clientCallbacks.Values)
					{
						callback.RefreshClients(clientList);
						callback.UserLeave(client);
						callback.UserEndDraft();
					}
				}
				return;
			}
		}

		#endregion
	}
}