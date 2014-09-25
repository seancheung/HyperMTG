using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HyperServer.Common
{
	[DataContract]
	public class Room : Entity
	{
		public Room()
		{
			Players = new List<Guid>();
		}

		[DataMember]
		public GameMode GameMode { get; set; }

		[DataMember]
		public GameFormat GameFormat { get; set; }

		[DataMember]
		public List<Guid> Players { get; set; }

		[DataMember]
		public string Desc { get; set; }

		[DataMember]
		public string Password { get; set; }

		[DataMember]
		public int RoomSize { get; set; }

		[DataMember]
		public bool Started { get; set; }

		public bool IsFull
		{
			get { return Players.Count >= RoomSize; }
		}
	}
}