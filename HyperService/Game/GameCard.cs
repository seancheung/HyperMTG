using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using HyperService.Common;

namespace HyperService.Game
{
	/// <summary>
	/// Card in game
	/// </summary>
	[DataContract]
	public class GameCard : Entity
	{
		public GameCard()
		{
			Counters = new Dictionary<CounterType, int>();
		}

		public GameCard(string cardID)
		{
			Counters = new Dictionary<CounterType, int>();
			CardID = cardID;
		}

		/// <summary>
		/// DB id of the card
		/// </summary>
		[DataMember]
		public string CardID { get; set; }

		/// <summary>
		/// Counters on the card
		/// </summary>
		[DataMember]
		public Dictionary<CounterType, int> Counters { get; private set; }

		/// <summary>
		/// Controller of the card
		/// </summary>
		[DataMember]
		public Guid Controller { get; set; }

		/// <summary>
		/// Owner of the card
		/// </summary>
		[DataMember]
		public Guid Owner { get; set; }

		/// <summary>
		/// Zone in which the card is in
		/// </summary>
		[DataMember]
		public Zone Zone { get; set; }

		/// <summary>
		/// Position of the card on board
		/// </summary>
		[DataMember]
		public double[] Position { get; set; }
	}
}