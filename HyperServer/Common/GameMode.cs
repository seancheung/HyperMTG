using System.Runtime.Serialization;

namespace HyperServer.Common
{
	[DataContract]
	public enum GameMode
	{
		[EnumMember] FreeForAll,
		[EnumMember] Draft,
		[EnumMember] Sealed,
		[EnumMember] Commander,
		[EnumMember] Planechase,
		[EnumMember] TwoHeadedGiant,
		[EnumMember] Archenemy
	}
}