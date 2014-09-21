using System;
using System.Runtime.Serialization;

namespace HyperServer.Common
{
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
}