using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using HyperKore.Logger;

namespace HyperService.Common
{
	public abstract class Service<TService, TContract, TCallbackContract> : IService
	{
		protected readonly object SyncObj = new object();
		protected ServiceHost Host;
		protected string IP;
		protected int Port;

		protected TCallbackContract CurrentCallback
		{
			get { return OperationContext.Current.GetCallbackChannel<TCallbackContract>(); }
		}

		#region IService Members

		/// <summary>
		/// Start Service
		/// </summary>
		/// <returns></returns>
		public virtual bool StartService()
		{
			Uri tcpAdrs = new Uri(string.Format("net.tcp://{0}:{1}/HyperService/", IP, Port));

			Uri httpAdrs = new Uri(string.Format("http://{0}:{1}/HyperService/", IP, Port + 1));

			Uri[] baseAdresses = {tcpAdrs, httpAdrs};

			if (Host != null)
			{
				Host.Close();
			}

			Host = new ServiceHost(
				typeof (TService), baseAdresses);

			NetTcpBinding tcpBinding =
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

			ServiceMetadataBehavior mBehave =
				new ServiceMetadataBehavior();
			Host.Description.Behaviors.Add(mBehave);

			Host.AddServiceEndpoint(typeof (IMetadataExchange),
				MetadataExchangeBindings.CreateMexTcpBinding(),
				string.Format("net.tcp://{0}:{1}/HyperService/mex", IP, Port - 1));

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

		/// <summary>
		/// Stop service
		/// </summary>
		/// <returns></returns>
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

		#endregion
	}

	public interface IService
	{
		bool StartService();
		bool StopService();
	}
}