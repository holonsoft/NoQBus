using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace holonsoft.NoQBus.Abstractions.Contracts
{
	/// <summary>
	/// Provides an interface for filtering messages
	/// Suggested usage: two scenarios.
	/// 1st it is used due bootstrapping of an application to make sure that only message of
	/// appropriate bootstrapping phase will be fired.
	/// 2nd usage is to get a complete send/receive protocol log that can be processed further / later. E. g. the ScriMesEngine
	/// <seealso cref="https://github.com/holonsoft/ScriMesEngine"/> uses this technology
	/// </summary>
	public interface IMessageBusFiltering
	{
		/// <summary>
		/// Add a filter for a request
		/// </summary>
		/// <typeparam name="TRequest">Request type, must support <see cref="IRequest"/></typeparam>
		/// <param name="filter">handler, returns true if message can be processed, otherwise false</param>
		/// <returns>A guid for this filter</returns>
		public Task<Guid> AddRequestFilter<TRequest>(Func<TRequest, Task<bool>> filter) 
			where TRequest : IRequest;

		/// <summary>
		/// Add a filter for a request
		/// </summary>
		/// <typeparam name="TRequest">Request type, must support <see cref="IRequest"/></typeparam>
		/// <param name="filter">A filter implementation that supports <see cref="IRequestFilter"/></param>
		/// <returns>A guid for this filter</returns>
		public Task<Guid> AddRequestFilter<TRequest>(IRequestFilter<TRequest> filter)
			where TRequest : IRequest
			=> AddRequestFilter<TRequest>(filter.Filter);


		/// <summary>
		/// Add a filter for a request
		/// </summary>
		/// <param name="filter">A filter implementation that supports <see cref="IRequestFilter"/></param>
		/// <returns>A guid for this filter</returns>
		public Task<Guid> AddRequestFilter(IRequestFilter filter)
			=> AddRequestFilter<IRequest>(filter.Filter);

		/// <summary>
		/// Add a filter for a request
		/// </summary>
		/// <typeparam name="TRequest">Request type, must support <see cref="IRequest"/></typeparam>
		/// <param name="filterFactory">A filter factory with an implementation of <see cref="IRequestFilter"/></param>
		/// <returns>A guid for this filter</returns>
		public Task<Guid> AddRequestFilter<TRequest>(Func<IRequestFilter<TRequest>> filterFactory)
			where TRequest : IRequest
			=> AddRequestFilter<TRequest>(request => filterFactory().Filter(request));


		/// <summary>
		/// Add a filter for a request
		/// </summary>
		/// <param name="filterFactory">A filter factory with an implementation of <see cref="IRequestFilter"/></param>
		/// <returns>A guid for this filter</returns>
		public Task<Guid> AddRequestFilter(Func<IRequestFilter> filterFactory)
			=> AddRequestFilter<IRequest>(request => filterFactory().Filter(request));


		/// <summary>
		/// Add a filter for responses
		/// </summary>
		/// <typeparam name="TResponse">Type of response, must support <see cref="IResponse"/></typeparam>
		/// <param name="filter">Handler for response array processing</param>
		/// <returns>A guid for this filter</returns>
		public Task<Guid> AddResponseFilter<TResponse>(Func<IEnumerable<TResponse>, Task<IEnumerable<TResponse>>> filter)
			where TResponse : IResponse;


		/// <summary>
		/// Add a filter for responses
		/// </summary>
		/// <typeparam name="TResponse">Type of response, must support <see cref="IResponse"/></typeparam>
		/// <param name="filter">Handler for response array processing, implements <see cref="IResponseFilter"/></param>
		/// <returns>A guid for this filter</returns>
		public Task<Guid> AddResponseFilter<TResponse>(IResponseFilter<TResponse> filter) 
			where TResponse : IResponse
			=> AddResponseFilter<TResponse>(filter.Filter);


		/// <summary>
		/// Add a filter for responses
		/// </summary>
		/// <param name="filter">Handler for response array processing, implements <see cref="IResponseFilter"/></param>
		/// <returns>A guid for this filter</returns>
		public Task<Guid> AddResponseFilter(IResponseFilter filter)
			=> AddResponseFilter<IResponse>(filter.Filter);


		/// <summary>
		/// Add a filter for responses
		/// </summary>
		/// <typeparam name="TResponse">Type of response, must support <see cref="IResponse"/></typeparam>
		/// <param name="filterFactory">Handler factory for response array processing, implements <see cref="IResponseFilter"/></param>
		/// <returns>A guid for this filter</returns>
		public Task<Guid> AddResponseFilter<TResponse>(Func<IResponseFilter<TResponse>> filterFactory) 
			where TResponse : IResponse
			=> AddResponseFilter<TResponse>(response => filterFactory().Filter(response));


		/// <summary>
		/// Add a filter for responses
		/// </summary>
		/// <param name="filterFactory">Handler factory for response array processing, implements <see cref="IResponseFilter"/></param>
		/// <returns>A guid for this filter</returns>
		public Task<Guid> AddResponseFilter(Func<IResponseFilter> filterFactory)
			=> AddResponseFilter<IResponse>(response => filterFactory().Filter(response));


		/// <summary>
		/// Remove a given filter for requests, identified by its GUID
		/// If id does not exist the function call we return without an exception
		/// </summary>
		/// <param name="requestFilterId">ID of filter</param>
		/// <returns>A simple task</returns>
		public Task RemoveRequestFilter(Guid requestFilterId);


		/// <summary>
		/// Remove a given filter for responses, identified by its GUID
		/// If id does not exist the function call we return without an exception
		/// </summary>
		/// <param name="responseFilterId">ID of filter</param>
		/// <returns>A simple task</returns>
		public Task RemoveResponseFilter(Guid responseFilterId);

	}
}
