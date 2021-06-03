using System;
using System.Threading.Tasks;

namespace holonsoft.NoQBus
{
	public interface IMessageBus
	{
		public Task<Guid> Subscribe<TRequest>(Func<TRequest, Task> action) where TRequest : IRequest;
		public Task<Guid> Subscribe(IConsumer consumer);
		public Task<Guid> Subscribe<TRequest>(IConsumer<TRequest> consumer) where TRequest : IRequest;
		public Task<Guid> Subscribe<TRequest>(Func<IConsumer> consumerFactory) where TRequest : IRequest;
		public Task<Guid> Subscribe<TRequest>(Func<IConsumer<TRequest>> consumerFactory) where TRequest : IRequest;


		public Task<Guid> Subscribe<TRequest, TResponse>(Func<TRequest, Task<TResponse>> action) where TRequest : IRequest
																																														 where TResponse : IResponse;
		public Task<Guid> Subscribe(IRespondToRequest respondToRequest);
		public Task<Guid> Subscribe<TRequest, TResponse>(IRespondToRequest<TRequest, TResponse> respondToRequest) where TRequest : IRequest
																																																							where TResponse : IResponse;
		public Task<Guid> Subscribe<TRequest, TResponse>(Func<IRespondToRequest> respondToRequestFactory) where TRequest : IRequest
																																																			where TResponse : IResponse;
		public Task<Guid> Subscribe<TRequest, TResponse>(Func<IRespondToRequest<TRequest, TResponse>> respondToRequestFactory) where TRequest : IRequest
																																																													 where TResponse : IResponse;

		public Task CancelSubscription(Guid subscriptionId);

		public Task Publish(IRequest request);

		public Task<IResponse[]> GetResponses(IRequest request);

		public Task<TResponse[]> GetResponses<TResponse>(IRequest request) where TResponse : IResponse;

		public Task<TResponse[]> GetResponses<TRequest, TResponse>(TRequest request) where TRequest : IRequest<TResponse>
																																								 where TResponse : IResponse<TRequest>;
	}
}
