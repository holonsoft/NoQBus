using holonsoft.NoQBus.Abstractions.Contracts;
using System;

namespace holonsoft.NoQBus.Abstractions.Models
{
	public abstract record MessageBase : IMessage
	{
		public string Culture { get; init; }

		public string SenderId { get; init; }

		public Guid MessageId { get; init; }

		public string SessionId { get; init; }

		public DateTime CreationTimeStamp { get; init; }

		private static readonly string _globalSenderId = Guid.NewGuid().ToString();

		public MessageBase()
		{
			Culture = System.Globalization.CultureInfo.CurrentUICulture.Name; //e.g. en-US, de-DE
			SenderId = _globalSenderId;
			MessageId = Guid.NewGuid();
			SessionId = "";
			CreationTimeStamp = DateTime.UtcNow;
		}

		public MessageBase(IMessage cloneFromMessage)
		{
			Culture = cloneFromMessage.Culture;
			SenderId = cloneFromMessage.SenderId;
			MessageId = Guid.NewGuid();
			SessionId = cloneFromMessage.SessionId;
			CreationTimeStamp = DateTime.UtcNow;
		}
	}
}
