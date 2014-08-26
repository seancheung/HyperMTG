using System.Runtime.Serialization;

namespace HyperService
{
	[DataContract]
	public class HyperAction
	{
		[DataMember]
		public Player Player { get; set; }
	}
}