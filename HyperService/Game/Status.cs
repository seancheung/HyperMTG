using System.Runtime.Serialization;

namespace HyperService.Game
{
	[DataContract]
	public enum CardStatus
	{
		[EnumMember] Tapped,
		[EnumMember] Flipped,
		[EnumMember] FaceDown,
		[EnumMember] PhasedOut
	}
}