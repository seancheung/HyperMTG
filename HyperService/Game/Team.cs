using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using HyperService.Common;

namespace HyperService.Game
{
	[DataContract]
	public class Team : Entity
	{
		public Team()
		{
			Players = new List<Guid>();
		}

		/// <summary>
		/// Players in the team
		/// </summary>
		[DataMember]
		public List<Guid> Players { get; private set; }

		/// <summary>
		/// Life of the team
		/// </summary>
		[DataMember]
		public int Life { get; set; }

		/// <summary>
		/// Poison counter
		/// </summary>
		[DataMember]
		public int PoisonCounter { get; set; }

		/// <summary>
		/// Max life
		/// </summary>
		[DataMember]
		public int MaxLife { get; set; }

		/// <summary>
		/// Current step
		/// </summary>
		[DataMember]
		public Step CurrentStep { get; set; }

		/// <summary>
		/// Current status
		/// </summary>
		[DataMember]
		public TeamStatus Status { get; set; }
	}
}