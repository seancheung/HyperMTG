using System.Runtime.Serialization;

namespace HyperService.Game
{
	[DataContract]
	public enum Step
	{
		[EnumMember] Untap,
		[EnumMember] Draw,
		[EnumMember] MainA,
		[EnumMember] CombatBegin,
		[EnumMember] DeclareAttackers,
	}
}