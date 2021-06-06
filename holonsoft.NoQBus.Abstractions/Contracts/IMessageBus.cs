using System;
using System.Linq;
using System.Threading.Tasks;

namespace holonsoft.NoQBus
{
	public interface IMessageBus
	{
		public Task<Guid> Subscribe<TRequest>(Func<TRequest, Task> action) where TRequest : IRequest;
		public Task<Guid> Subscribe<TRequest, TResponse>(Func<TRequest, Task<TResponse>> action) where TRequest : IRequest
																																														 where TResponse : IResponse;
		public Task CancelSubscription(Guid subscriptionId);
		public Task Publish(IRequest request);
		public Task<IResponse[]> GetResponses(IRequest request);

		public Task<Guid> Subscribe(IConsumer consumer)
			=> Subscribe<IRequest>(consumer.Consume);
		public Task<Guid> Subscribe<TRequest>(IConsumer<TRequest> consumer) where TRequest : IRequest
			=> Subscribe<TRequest>(consumer.Consume);
		public Task<Guid> Subscribe<TRequest>(Func<IConsumer> consumerFactory) where TRequest : IRequest
			=> Subscribe<TRequest>(req => consumerFactory().Consume(req));
		public Task<Guid> Subscribe<TRequest>(Func<IConsumer<TRequest>> consumerFactory) where TRequest : IRequest
			=> Subscribe<TRequest>(req => consumerFactory().Consume(req));

		public Task<Guid> Subscribe(IRespondToRequest respondToRequest)
			=> Subscribe<IRequest, IResponse>(respondToRequest.Respond);
		public Task<Guid> Subscribe<TRequest, TResponse>(IRespondToRequest<TRequest, TResponse> respondToRequest) where TRequest : IRequest
																																																							where TResponse : IResponse
			=> Subscribe<TRequest, TResponse>(respondToRequest.Respond);
		public Task<Guid> Subscribe<TRequest, TResponse>(Func<IRespondToRequest> respondToRequestFactory) where TRequest : IRequest
																																																			where TResponse : IResponse
			=> Subscribe<TRequest, IResponse>(req => respondToRequestFactory().Respond(req));
		public Task<Guid> Subscribe<TRequest, TResponse>(Func<IRespondToRequest<TRequest, TResponse>> respondToRequestFactory) where TRequest : IRequest
																																																													 where TResponse : IResponse
			=> Subscribe<TRequest, TResponse>(req => respondToRequestFactory().Respond(req));

		public async Task<TResponse[]> GetResponses<TResponse>(IRequest request) where TResponse : IResponse
			=> (await GetResponses(request)).OfType<TResponse>().ToArray();

		public async Task<IResponse> GetResponse(IRequest request)
			=> (await GetResponses(request)).FirstOrDefault();

		public async Task<TResponse> GetResponse<TResponse>(IRequest request) where TResponse : IResponse
			=> (await GetResponses(request)).OfType<TResponse>().FirstOrDefault();
	}
}
