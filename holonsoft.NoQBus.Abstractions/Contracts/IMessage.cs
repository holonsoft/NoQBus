using System;

namespace holonsoft.NoQBus
{
	public interface IMessage
	{
		public string Culture { get; init; }
		public string SenderId { get; init; }
		public Guid MessageId { get; init; }
		public string AuthToken { get; init; }
	}
}
