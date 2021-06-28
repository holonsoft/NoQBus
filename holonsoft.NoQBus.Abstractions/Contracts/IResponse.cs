using System;

namespace holonsoft.NoQBus.Abstractions.Contracts
{
	public interface IResponse : IMessage
	{
#if NET5_0_OR_GREATER
		public Guid CorrospondingRequestMessageId { get; init; }
#else
		public Guid CorrospondingRequestMessageId { get; set; }
#endif
	}

	public interface IResponse<TRequest> : IResponse where TRequest : IRequest
	{

	}
}
