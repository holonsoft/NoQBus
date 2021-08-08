using holonsoft.NoQBus.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace holonsoft.NoQBus
{
	public partial class MessageBus : IMessageBus, IRemoteMessageBus, IMessageBusFiltering
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

		Task<Guid> IMessageBusFiltering.AddRequestFilter<TRequest>(Func<TRequest, Task<bool>> filter)
			=> AddRequestFilter<TRequest>(filter);

		Task IMessageBusFiltering.RemoveRequestFilter(Guid requestFilterId)
			=> RemoveRequestFilter(requestFilterId);

		Task<Guid> IMessageBusFiltering.AddResponseFilter<TResponse>(Func<IEnumerable<TResponse>, Task<IEnumerable<TResponse>>> filter)
			=> AddResponseFilter<TResponse>(filter);

		Task IMessageBusFiltering.RemoveResponseFilter(Guid responseFilterId)
			=> RemoveResponseFilter(responseFilterId);
	}
}
