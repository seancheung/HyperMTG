using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using HyperKore.Logger;

namespace HyperServer.Common
{
	public abstract class Service<TContract, TCallbackContract>
	{
		protected readonly object SyncObj = new object();
		protected ServiceHost Host;
		protected string IP;
		protected int Port;

		protected TCallbackContract CurrentCallback
		{
			get { return OperationContext.Current.GetCallbackChannel<TCallbackContract>(); }
		}

		public virtual bool StartService()
		{
			var tcpAdrs = new Uri(string.Format("net.tcp://{0}:{1}/HyperServer/", IP, Port));

			var httpAdrs = new Uri(string.Format("http://{0}:{1}/HyperServer/", IP, Port + 1));

			Uri[] baseAdresses = {tcpAdrs, httpAdrs};

			if (Host != null)
			{
				Host.Close();
			}

			Host = new ServiceHost(
				GetType(), baseAdresses);

			var tcpBinding =
				new NetTcpBinding(SecurityMode.None, true);

			tcpBinding.MaxConnections = 100;

			ServiceThrottlingBehavior throttle;
			throttle =
				Host.Description.Behaviors.Find<ServiceThrottlingBehavior>();
			if (throttle == null)
			{
				throttle = new ServiceThrottlingBehavior();
				throttle.MaxConcurrentCalls = 100;
				throttle.MaxConcurrentSessions = 100;
				Host.Description.Behaviors.Add(throttle);
			}

			tcpBinding.ReceiveTimeout = new TimeSpan(20, 0, 0);
			tcpBinding.ReliableSession.Enabled = true;
			tcpBinding.ReliableSession.InactivityTimeout =
				new TimeSpan(20, 0, 10);

			Host.AddServiceEndpoint(typeof (TContract),
				tcpBinding, "tcp");

			var mBehave =
				new ServiceMetadataBehavior();
			Host.Description.Behaviors.Add(mBehave);

			Host.AddServiceEndpoint(typeof (IMetadataExchange),
				MetadataExchangeBindings.CreateMexTcpBinding(),
				string.Format("net.tcp://{0}:{1}/HyperServer/mex", IP, Port - 1));

			try
			{
				Host.Open();
				return true;
			}
			catch (Exception ex)
			{
				Logger.Log(ex, this);
				return false;
			}
		}

		public virtual bool StopService()
		{
			if (Host != null)
			{
				try
				{
					Host.Close();
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
	}
}