using System.Collections.Generic;
using System.Runtime.Serialization;
using HyperServer.Common;

namespace HyperServer.Draft
{
	[DataContract]
	public class DraftPlayer : Client
	{
		public DraftPlayer()
		{
			PoolList = new List<string>();
			HandList = new List<string>();
		}

		public DraftPlayer(string name) : base(name)
		{
			PoolList = new List<string>();
			HandList = new List<string>();
		}

		[DataMember]
		public bool IsReady { get; set; }

		[DataMember]
		public List<string> PoolList { get; set; }

		[DataMember]
		public List<string> HandList { get; set; }
	}
}