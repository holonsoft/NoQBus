using System;

namespace holonsoft.NoQBus.Abstractions.Contracts
{
	public interface IResponse : IMessage
	{
		public Guid CorrespondingRequestMessageId { get; init; }
	}

	public interface IResponse<TRequest> : IResponse 
		where TRequest : IRequest
	{

	}
}
