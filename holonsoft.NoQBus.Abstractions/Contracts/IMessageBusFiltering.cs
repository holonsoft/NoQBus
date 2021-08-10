using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace holonsoft.NoQBus.Abstractions.Contracts
{
	public interface IMessageBusFiltering
	{
		public Task<Guid> AddRequestFilter<TRequest>(Func<TRequest, Task<bool>> filter) 
			where TRequest : IRequest;

		public Task RemoveRequestFilter(Guid requestFilterId);

		public Task<Guid> AddResponseFilter<TResponse>(Func<IEnumerable<TResponse>, Task<IEnumerable<TResponse>>> filter) 
			where TResponse : IResponse;

		public Task RemoveResponseFilter(Guid responseFilterId);

		public Task<Guid> AddRequestFilter<TRequest>(IRequestFilter<TRequest> filter) 
			where TRequest : IRequest
			=> AddRequestFilter<TRequest>(filter.Filter);

		public Task<Guid> AddRequestFilter(IRequestFilter filter)
			=> AddRequestFilter<IRequest>(filter.Filter);

		public Task<Guid> AddRequestFilter<TRequest>(Func<IRequestFilter<TRequest>> filterFactory) 
			where TRequest : IRequest
			=> AddRequestFilter<TRequest>(request => filterFactory().Filter(request));

		public Task<Guid> AddRequestFilter(Func<IRequestFilter> filterFactory)
			=> AddRequestFilter<IRequest>(request => filterFactory().Filter(request));

		public Task<Guid> AddResponseFilter<TResponse>(IResponseFilter<TResponse> filter) 
			where TResponse : IResponse
			=> AddResponseFilter<TResponse>(filter.Filter);

		public Task<Guid> AddResponseFilter(IResponseFilter filter)
			=> AddResponseFilter<IResponse>(filter.Filter);

		public Task<Guid> AddResponseFilter<TResponse>(Func<IResponseFilter<TResponse>> filterFactory) 
			where TResponse : IResponse
			=> AddResponseFilter<TResponse>(response => filterFactory().Filter(response));

		public Task<Guid> AddResponseFilter(Func<IResponseFilter> filterFactory)
			=> AddResponseFilter<IResponse>(response => filterFactory().Filter(response));

	}
}
