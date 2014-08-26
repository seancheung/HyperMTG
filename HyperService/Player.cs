using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HyperService
{
	[DataContract]
	public class Player
	{
		[DataMember]
		public string ID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public DateTime Time { get; set; }

		[DataMember]
		public int Life { get; set; }

		[DataMember]
		public int Poison { get; set; }

		[DataMember]
		public bool InTurn { get; set; }

		[DataMember]
		public List<Kard> Library { get; set; }

		[DataMember]
		public List<Kard> Hand { get; set; }

		[DataMember]
		public List<Kard> Graveyard { get; set; }

		[DataMember]
		public List<Kard> Exile { get; set; }

		[DataMember]
		public List<Kard> Battlefiled { get; set; }
	}
}