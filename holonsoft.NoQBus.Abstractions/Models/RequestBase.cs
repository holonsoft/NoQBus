using holonsoft.NoQBus.Abstractions.Contracts;

namespace holonsoft.NoQBus.Abstractions.Models
{

	public abstract record RequestBase : MessageBase, IRequest
	{
		protected RequestBase() : base()
		{
		}

		protected RequestBase(IMessage cloneFromMessage) : base(cloneFromMessage)
		{
		}
	}

	public abstract record RequestBase<TResponse> : RequestBase, IRequest<TResponse> where TResponse : IResponse
	{
		public RequestBase() : base()
		{
		}

		public RequestBase(IMessage cloneFromMessage) : base(cloneFromMessage)
		{
		}
	}
}
