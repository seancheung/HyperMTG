using System.Runtime.Serialization;

namespace HyperService.Game
{
	[DataContract]
	public enum Zone
	{
		[EnumMember] Library,
		[EnumMember] Hand,
		[EnumMember] Battlefield,
		[EnumMember] Exile,
		[EnumMember] Graveyard,
		[EnumMember] Stack,
		[EnumMember] Command,
	}
}