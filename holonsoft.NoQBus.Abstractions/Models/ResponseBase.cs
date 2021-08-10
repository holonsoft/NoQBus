using holonsoft.NoQBus.Abstractions.Contracts;
using System;

namespace holonsoft.NoQBus.Abstractions.Models
{
	/// <summary>
	/// Base record structure for a response
	/// </summary>
	public abstract record ResponseBase : MessageBase, IResponse
	{
		/// <summary>
		/// Weak relation between request and response
		/// </summary>
		public Guid CorrespondingRequestMessageId { get; init; }

		/// <summary>
		/// Standard constructor
		/// </summary>
		protected ResponseBase()
		{
		}

		/// <summary>
		/// Create a response based on basic request infos
		/// </summary>
		/// <param name="cloneFromMessageButNewTimestampAndGuid"></param>
		protected ResponseBase(IMessage cloneFromMessageButNewTimestampAndGuid) 
			: base(cloneFromMessageButNewTimestampAndGuid)
		{
			CorrespondingRequestMessageId = cloneFromMessageButNewTimestampAndGuid.MessageId;
		}
	}

	/// <summary>
	/// Base record structure for a response
	/// </summary>
	public abstract record ResponseBase<TRequest> : ResponseBase, IResponse<TRequest> 
		where TRequest : IRequest
	{
		/// <summary>
		/// Standard constructor
		/// </summary>
		protected ResponseBase()
		{
		}

		/// <summary>
		/// Create a response based on basic request infos
		/// </summary>
		/// <param name="cloneFromMessageButNewTimestampAndGuid"></param>
		protected ResponseBase(TRequest cloneFromMessageButNewTimestampAndGuid) 
			: base(cloneFromMessageButNewTimestampAndGuid)
		{
		}
	}
}
