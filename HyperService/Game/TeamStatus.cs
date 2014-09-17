using System.Runtime.Serialization;

namespace HyperService.Game
{
	[DataContract]
	public enum TeamStatus
	{
		[EnumMember] Normal,
		[EnumMember] Win,
		[EnumMember] Lose
	}
}