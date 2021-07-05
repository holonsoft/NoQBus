using holonsoft.NoQBus.Abstractions.Contracts;
using System;

namespace holonsoft.NoQBus.Abstractions.Models
{
	public abstract record MessageBase : IMessage
	{
#if NET5_0_OR_GREATER
		public string Culture { get; init; }

		public string SenderId { get; init; }

		public Guid MessageId { get; init; }

		public string AuthToken { get; init; }

		public DateTime CreationTimeStamp { get; init; }
#else
		public string Culture { get; set; }

		public string SenderId { get; set; }

		public Guid MessageId { get; set; }

		public string AuthToken { get; set; }

		public DateTime CreationTimeStamp { get; set; }
#endif

		private static readonly string _globalSenderId = Guid.NewGuid().ToString();

		public MessageBase()
		{
			Culture = System.Globalization.CultureInfo.CurrentUICulture.Name; //e.g. en-US, de-DE
			SenderId = _globalSenderId;
			MessageId = Guid.NewGuid();
			AuthToken = "";
			CreationTimeStamp = DateTime.UtcNow;
		}

		public MessageBase(IMessage cloneFromMessage)
		{
			Culture = cloneFromMessage.Culture;
			SenderId = cloneFromMessage.SenderId;
			MessageId = Guid.NewGuid();
			AuthToken = cloneFromMessage.AuthToken;
			CreationTimeStamp = DateTime.UtcNow;
		}
	}
}
