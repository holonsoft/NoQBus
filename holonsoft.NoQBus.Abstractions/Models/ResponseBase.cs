using System;

namespace holonsoft.NoQBus
{

	public abstract record ResponseBase : MessageBase, IResponse
	{
		public Guid CorrospondingRequestMessageId { get; init; }

		public ResponseBase()
		{
		}

		public ResponseBase(IMessage cloneFromMessage) : base(cloneFromMessage)
		{
			CorrospondingRequestMessageId = cloneFromMessage.MessageId;
		}
	}

	public abstract record ResponseBase<TRequest> : ResponseBase, IResponse<TRequest> where TRequest : IRequest
	{
		public ResponseBase()
		{
		}

		public ResponseBase(TRequest cloneFromRequest) : base(cloneFromRequest)
		{
		}
	}
}
