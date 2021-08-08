using System;

namespace holonsoft.NoQBus.Abstractions.Contracts
{
	public interface IMessage
	{
		public string Culture { get; init; }
		public string SenderId { get; init; }
		public Guid MessageId { get; init; }
		public string SessionId { get; init; }
	}
}
