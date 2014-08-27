using System;

namespace HyperService
{
	public class HyperEventArgs : EventArgs
	{
		public HyperAction Action;
		public string Message;
		public MessageType MsgType;
		public Player Player;
		public Team Team;
	}
}