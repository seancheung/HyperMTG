using System.Runtime.Serialization;

namespace HyperService.Game
{
	[DataContract]
	public enum ActionType
	{
		[EnumMember] Move,
		[EnumMember] Tap,
		[EnumMember] Flip
	}
}