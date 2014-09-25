using System.Runtime.Serialization;

namespace HyperServer.Common
{
	[DataContract]
	public enum JoinRoomResult
	{
		[EnumMember] Success,
		[EnumMember] RoomFull,
		[EnumMember] IncorrectPassword,
		[EnumMember] Unkown
	}
}