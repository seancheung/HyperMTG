using System.Runtime.Serialization;

namespace HyperService.Game
{
	[DataContract]
	public enum GameMode
	{
		[EnumMember] FreeForAll,
		[EnumMember] TwoHeadedGiant,
		[EnumMember] Commander,
		[EnumMember] PlaneChase,
		[EnumMember] Archenemy
	}
}