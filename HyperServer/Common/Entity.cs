using System;
using System.Runtime.Serialization;

namespace HyperServer.Common
{
	[DataContract]
	public class Entity
	{
		public Entity()
		{
			ID = Guid.NewGuid();
		}

		[DataMember]
		public Guid ID { get; private set; }
	}
}