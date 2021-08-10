using System;

namespace holonsoft.NoQBus.Abstractions.Contracts
{
	/// <summary>
	/// Base interface for response messages
	/// </summary>
	public interface IResponse : IMessage
	{
		/// <summary>
		/// Can be used to set a weak relation between a request and its responses
		/// </summary>
		public Guid CorrespondingRequestMessageId { get; init; }
	}


	/// <summary>
	/// Base interface for response messages
	/// </summary>
	public interface IResponse<TRequest> : IResponse 
		where TRequest : IRequest
	{

	}
}
