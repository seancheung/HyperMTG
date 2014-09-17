using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using HyperService.Common;

namespace HyperService.Game
{
	[DataContract]
	public class Player : Client
	{
		public Player()
		{
			Cards = new Dictionary<Zone, List<Guid>>();
		}

		public Player(string name) : base(name)
		{
			Cards = new Dictionary<Zone, List<Guid>>();
		}

		/// <summary>
		/// Team ID of the player
		/// </summary>
		[DataMember]
		public Guid TeamID { get; set; }

		/// <summary>
		/// Maximum hand size of the player
		/// </summary>
		[DataMember]
		public int MaxHandSize { get; set; }

		/// <summary>
		/// Cards of the player
		/// </summary>
		[DataMember]
		public Dictionary<Zone, List<Guid>> Cards { get; private set; }
	}
}