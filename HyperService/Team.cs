using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HyperService
{
	[DataContract]
	public class Team
	{
		[DataMember]
		public string ID { get; set; }

		[DataMember]
		public int Life { get; set; }

		[DataMember]
		public int Poison { get; set; }

		[DataMember]
		public bool InTurn { get; set; }

		[DataMember]
		public List<Player> Players { get; set; }
	}
}