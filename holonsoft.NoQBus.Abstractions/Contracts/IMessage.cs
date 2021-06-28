using System;

namespace holonsoft.NoQBus.Abstractions.Contracts
{
	public interface IMessage
	{
#if NET5_0_OR_GREATER
		public string Culture { get; init; }
		public string SenderId { get; init; }
		public Guid MessageId { get; init; }
		public string AuthToken { get; init; }
#else
		public string Culture { get; set; }
		public string SenderId { get; set; }
		public Guid MessageId { get; set; }
		public string AuthToken { get; set; }
#endif
	}
}
