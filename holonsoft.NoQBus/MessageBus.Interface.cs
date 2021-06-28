using holonsoft.NoQBus.Abstractions.Contracts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace holonsoft.NoQBus
{
	public partial class MessageBus : IMessageBus, IRemoteMessageBus
	{
		Task<Guid> IMessageBus.Subscribe<TRequest>(Func<TRequest, Task> action)
			 => Subscribe<TRequest>(action);

		Task<Guid> IMessageBus.Subscribe<TRequest, TResponse>(Func<TRequest, Task<TResponse>> action)
			 => Subscribe(action);

		Task IMessageBus.CancelSubscription(Guid subscriptionId)
			 => CancelSubscription(subscriptionId);

		Task IMessageBus.Publish(IRequest request)
				 => Publish(request);

		Task<IResponse[]> IMessageBus.GetResponses(IRequest request)
			 => GetResponses(request);

		Task<IResponse[]> IRemoteMessageBus.GetResponsesForRemotedRequest(IRequest request)
			 => GetResponses(request, isRemoteCall: true);
	}
}
