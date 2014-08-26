using System;
using System.Runtime.Serialization;

namespace HyperService
{
	[DataContract]
	public class Message
	{
		[DataMember]
		public Player Sender { get; set; }

		[DataMember]
		public object Content { get; set; }

		[DataMember]
		public DateTime Time { get; set; }
	}
}