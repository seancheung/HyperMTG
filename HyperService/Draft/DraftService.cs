using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;
using HyperKore.Logger;

namespace HyperService.Draft
{
	[ServiceContract(CallbackContract = typeof (IDraftCallBack), SessionMode = SessionMode.Required)]
	public interface IDraft
	{
		[OperationContract(IsInitiating = true)]
		bool Connect(Client client);

		[OperationContract(IsOneWay = true)]
		void Say(Message msg);

		[OperationContract(IsOneWay = true)]
		void Pick(Client client, int index);

		[OperationContract(IsOneWay = true,
			IsTerminating = true)]
		void Disconnect(Client client);
	}

	public interface IDraftCallBack
	{
		[OperationContract(IsOneWay = true)]
		void RefreshClients(List<Client> clients);

		[OperationContract(IsOneWay = true)]
		void Receive(Message msg);

		[OperationContract(IsOneWay = true)]
		void UserJoin(Client client);

		[OperationContract(IsOneWay = true)]
		void UserLeave(Client client);

		[OperationContract(IsOneWay = true)]
		void Picked(Client client, int index);
	}

	[DataContract]
	public class Client
	{
		[DataMember]
		public Guid ID { get; private set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public bool IsDone { get; set; }

		[DataMember]
		public bool IsBot { get; set; }

		public Client()
		{
			ID = Guid.NewGuid();
		}

		public Client(string name)
		{
			Name = name;
			ID = Guid.NewGuid();
		}
	}

	[DataContract]
	public class Message
	{
		[DataMember]
		public Guid Sender { get; set; }

		[DataMember]
		public string Content { get; set; }

		[DataMember]
		public DateTime Time { get; set; }
	}

	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
		ConcurrencyMode = ConcurrencyMode.Multiple,
		UseSynchronizationContext = false)]
	public class DraftService : IDraft
	{
		private readonly List<Client> clientList = new List<Client>();
		private readonly Dictionary<Client, IDraftCallBack> clientCallbacks = new Dictionary<Client, IDraftCallBack>();
		private readonly object syncObj = new object();
		private ServiceHost host;

		public IDraftCallBack CurrentCallback
		{
			get { return OperationContext.Current.GetCallbackChannel<IDraftCallBack>(); }
		}

		public bool Start(string ip, int port)
		{
			var tcpAdrs = new Uri(string.Format("net.tcp://{0}:{1}/HyperService.Draft/", ip, port));

			var httpAdrs = new Uri(string.Format("http://{0}:{1}/HyperService.Draft/", ip, port + 1));

			Uri[] baseAdresses = {tcpAdrs, httpAdrs};

			host = new ServiceHost(
				typeof (DraftService), baseAdresses);

			var tcpBinding =
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

			var mBehave =
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

		public bool Stop()
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

		public bool Connect(Client client)
		{
			if (!clientCallbacks.ContainsValue(CurrentCallback) && clientCallbacks.All(c => c.Key.ID != client.ID))
			{
				lock (syncObj)
				{
					clientCallbacks.Add(client, CurrentCallback);
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

		public void Say(Message msg)
		{
			lock (syncObj)
			{
				foreach (IDraftCallBack callback in clientCallbacks.Values)
				{
					callback.Receive(msg);
				}
			}
		}

		public void Pick(Client client, int index)
		{
			throw new NotImplementedException();
		}

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
					}
				}
				return;
			}
		}

		#endregion
	}
}