namespace holonsoft.NoQBus
{

	public abstract record RequestBase : MessageBase, IRequest
	{
		public RequestBase() : base()
		{
		}

		public RequestBase(IMessage cloneFromMessage) : base(cloneFromMessage)
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
