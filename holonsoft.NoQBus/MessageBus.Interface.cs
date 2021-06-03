using System;
using System.Linq;
using System.Threading.Tasks;

namespace holonsoft.NoQBus
{
	public partial class MessageBus : IMessageBus, IRemoteMessageBus
	{
		Task<Guid> IMessageBus.Subscribe(IConsumer consumer)
			=> Subscribe<IRequest>(consumer.Consume);
		Task<Guid> IMessageBus.Subscribe<TRequest>(IConsumer<TRequest> consumer)
			=> Subscribe<TRequest>(consumer.Consume);
		Task<Guid> IMessageBus.Subscribe<TRequest>(Func<IConsumer> consumerFactory)
			=> Subscribe<TRequest>(req => consumerFactory().Consume(req));
		Task<Guid> IMessageBus.Subscribe<TRequest>(Func<IConsumer<TRequest>> consumerFactory)
			=> Subscribe<TRequest>(req => consumerFactory().Consume(req));

		Task<Guid> IMessageBus.Subscribe(IRespondToRequest respondToRequest)
			=> Subscribe<IRequest, IResponse>(respondToRequest.Respond);
		Task<Guid> IMessageBus.Subscribe<TRequest, TResponse>(IRespondToRequest<TRequest, TResponse> respondToRequest)
			=> Subscribe<TRequest, TResponse>(respondToRequest.Respond);
		Task<Guid> IMessageBus.Subscribe<TRequest, TResponse>(Func<IRespondToRequest> respondToRequestFactory)
			=> Subscribe<TRequest, IResponse>(req => respondToRequestFactory().Respond(req));
		Task<Guid> IMessageBus.Subscribe<TRequest, TResponse>(Func<IRespondToRequest<TRequest, TResponse>> respondToRequestFactory)
			=> Subscribe<TRequest, TResponse>(req => respondToRequestFactory().Respond(req));

		async Task<TResponse[]> IMessageBus.GetResponses<TResponse>(IRequest request)
			=> (await GetResponses(request)).OfType<TResponse>().ToArray();

		async Task<TResponse[]> IMessageBus.GetResponses<TRequest, TResponse>(TRequest request)
			=> (await GetResponses(request)).OfType<TResponse>().ToArray();

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
