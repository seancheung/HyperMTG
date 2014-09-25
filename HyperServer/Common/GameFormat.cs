using System.Runtime.Serialization;

namespace HyperServer.Common
{
	[DataContract]
	public enum GameFormat
	{
		[EnumMember] Standard,
		[EnumMember] Modern,
		[EnumMember] Classic,
		[EnumMember] Vintage,
		[EnumMember] Legacy,
		[EnumMember] Free
	}
}