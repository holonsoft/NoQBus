using System;

namespace holonsoft.NoQBus.Abstractions.Contracts
{
	public interface IResponse : IMessage
	{
		public Guid CorrospondingRequestMessageId { get; init; }
	}

	public interface IResponse<TRequest> : IResponse where TRequest : IRequest
	{

	}
}
