using System;
using System.Runtime.Serialization;

namespace HyperService.Draft
{
	[DataContract]
	public class Client
	{
		/// <summary>
		/// Client ID
		/// </summary>
		[DataMember]
		public Guid ID { get; private set; }

		/// <summary>
		/// Client name
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Client finished current action
		/// </summary>
		[DataMember]
		public bool IsDone { get; set; }

		/// <summary>
		/// Whether its a bot
		/// </summary>
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
}