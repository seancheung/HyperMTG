using System;
using System.Collections.Generic;

namespace HyperKore.Game
{
	public enum MessageType
	{
	}

	public class Message
	{
		public Message(Guid sender, Guid receiver, MessageType msg, object info = null)
		{
			Info = info;
			DispatchTime = DateTime.MinValue;
			Msg = msg;
			Receiver = receiver;
			Sender = sender;
		}

		public Guid Sender { get; private set; }
		public Guid Receiver { get; private set; }
		public MessageType Msg { get; private set; }
		public DateTime DispatchTime { get; set; }
		public object Info { get; private set; }
	}

	public class MessageDispatcher
	{
		public static readonly MessageDispatcher Instance = new MessageDispatcher();
		private readonly Queue<Message> messages;

		private MessageDispatcher()
		{
			messages = new Queue<Message>();
		}

		private void Discharge(IEntity receiver, Message msg)
		{
			if (receiver != null)
			{
				receiver.HandleMessage(msg);
			}
		}

		public void DispatchMessage(double delay, Guid sender, Guid receiver, MessageType msg, object info)
		{
			IEntity receiverEntity = EntityManager.Instance.GetEntity(receiver);

			Message message = new Message(sender, receiver, msg, info);

			if (delay <= 0.01)
			{
				Discharge(receiverEntity, message);
			}
			else
			{
				message.DispatchTime = DateTime.Now.AddMilliseconds(delay);
				messages.Enqueue(message);
				DispatchDelayedMessages();
			}
		}

		public void DispatchDelayedMessages()
		{
			while (messages.Peek().DispatchTime > DateTime.Now)
			{
				Message msg = messages.Dequeue();
				IEntity receiver = EntityManager.Instance.GetEntity(msg.Receiver);
				Discharge(receiver, msg);
			}
		}
	}
}