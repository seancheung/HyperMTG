using System.Runtime.Serialization;

namespace HyperServer.Common
{
	[DataContract]
	public class Client : Entity
	{
		public Client()
		{
		}

		public Client(string name)
		{
			Name = name;
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public bool IsBot { get; set; }

		[DataMember]
		public bool IsHost { get; set; }
	}
}