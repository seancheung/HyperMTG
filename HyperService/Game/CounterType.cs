using System.Runtime.Serialization;

namespace HyperService.Game
{
	[DataContract]
	public enum CounterType
	{
		[EnumMember] Positive,
		[EnumMember] Negative,
		[EnumMember] Loyalty,
		[EnumMember] Poison,
		[EnumMember] MarkableA,
		[EnumMember] MarkableB,
		[EnumMember] MarkableC,
		[EnumMember] MarkableD,
		[EnumMember] MarkableE
	}
}