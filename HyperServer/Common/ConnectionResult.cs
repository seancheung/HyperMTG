using System.Runtime.Serialization;

namespace HyperServer.Common
{
	[DataContract]
	public enum ConnectionResult
	{
		[EnumMember] Success,
		[EnumMember] RoomFull,
		[EnumMember] AlreadyStarted,
		[EnumMember] UserExist,
		[EnumMember] Banned,
		[EnumMember] Unkown
	}
}