namespace holonsoft.NoQBus.Abstractions.Contracts;
public interface IMessageBus
{
  /// <summary>
  /// Subscribe to a fire-and-forget-request and provide a handler for it.
  /// </summary>
  /// <typeparam name="TRequest">request to be handled</typeparam>
  /// <param name="action">handler routine</param>
  /// <returns>An ID for this subscription</returns>
  public Task<Guid> Subscribe<TRequest>(Func<TRequest, Task> action)
    where TRequest : IRequest;


  /// <summary>
  /// Subscribe to request/response messages and provide a handler for it
  /// </summary>
  /// <typeparam name="TRequest">Request to be sent</typeparam>
  /// <typeparam name="TResponse">An expected response message</typeparam>
  /// <param name="action">handler routine</param>
  /// <returns>An ID for this subscription</returns>
  public Task<Guid> Subscribe<TRequest, TResponse>(Func<TRequest, Task<TResponse>> action)
    where TRequest : IRequest
    where TResponse : IResponse;


  /// <summary>
  /// Subscribe to a fire-and-forget-request and provide a consumer/handler for it.
  /// </summary>
  /// <param name="consumer">Consumer implementation</param>
  /// <returns>An ID for this subscription</returns>
  public Task<Guid> Subscribe(IConsumer consumer)
    => Subscribe<IRequest>(consumer.Consume);


  /// <summary>
  /// Subscribe to a fire-and-forget-request and provide a typesafe consumer/handler for it.
  /// </summary>
  /// <typeparam name="TRequest">Request to be handled</typeparam>
  /// <param name="consumer">Consumer implementation for this type of request</param>
  /// <returns>An ID for this subscription</returns>
  public Task<Guid> Subscribe<TRequest>(IConsumer<TRequest> consumer)
    where TRequest : IRequest
    => Subscribe<TRequest>(consumer.Consume);


  /// <summary>
  /// Subscribe to a fire-and-forget-request and provide a consumer/handler factory for it.
  /// </summary>
  /// <typeparam name="TRequest">Request to be handled</typeparam>
  /// <param name="consumerFactory">A factory that creates a handler on the fly</param>
  /// <returns>An ID for this subscription</returns>
  public Task<Guid> Subscribe<TRequest>(Func<IConsumer> consumerFactory)
    where TRequest : IRequest
    => Subscribe<TRequest>(req => consumerFactory().Consume(req));


  /// <summary>
  /// Subscribe to a fire-and-forget-request and provide a consumer/handler factory for it.
  /// </summary>
  /// <typeparam name="TRequest">Request to be handled</typeparam>
  /// <param name="consumerFactory">A factory that creates a handler on the fly</param>
  /// <returns>An ID for this subscription</returns>
  public Task<Guid> Subscribe<TRequest>(Func<IConsumer<TRequest>> consumerFactory)
    where TRequest : IRequest
    => Subscribe<TRequest>(req => consumerFactory().Consume(req));


  /// <summary>
  /// Subscribe to a responsible request message
  /// </summary>
  /// <param name="respondToRequest">Response to a request</param>
  /// <returns>An ID for this subscription</returns>
  public Task<Guid> Subscribe(IRespondToRequest respondToRequest)
    => Subscribe<IRequest, IResponse>(respondToRequest.Respond);

  /// <summary>
  /// Subscribe to a responsible request message
  /// </summary>
  /// <typeparam name="TRequest">Request message</typeparam>
  /// <typeparam name="TResponse">Response message</typeparam>
  /// <param name="respondToRequest"></param>
  /// <returns>An ID for this subscription</returns>
  public Task<Guid> Subscribe<TRequest, TResponse>(IRespondToRequest<TRequest, TResponse> respondToRequest)
    where TRequest : IRequest
    where TResponse : IResponse
    => Subscribe<TRequest, TResponse>(respondToRequest.Respond);

  /// <summary>
  /// Subscribe to a responsible request message
  /// </summary>
  /// <typeparam name="TRequest">Request message</typeparam>
  /// <typeparam name="TResponse">Response message</typeparam>
  /// <param name="respondToRequestFactory"></param>
  /// <returns>An ID for this subscription</returns>
  public Task<Guid> Subscribe<TRequest, TResponse>(Func<IRespondToRequest> respondToRequestFactory)
    where TRequest : IRequest
    where TResponse : IResponse
    => Subscribe<TRequest, IResponse>(req => respondToRequestFactory().Respond(req));

  /// <summary>
  /// Subscribe to a responsible request message within a factory provider for handling
  /// </summary>
  /// <typeparam name="TRequest">Request message</typeparam>
  /// <typeparam name="TResponse">Response message</typeparam>
  /// <param name="respondToRequestFactory">Factory for on-the-fly creation of a handler</param>
  /// <returns>An ID for this subscription</returns>
  public Task<Guid> Subscribe<TRequest, TResponse>(Func<IRespondToRequest<TRequest, TResponse>> respondToRequestFactory)
    where TRequest : IRequest
    where TResponse : IResponse
    => Subscribe<TRequest, TResponse>(req => respondToRequestFactory().Respond(req));


  /// <summary>
  /// Revoke a subscription.
  /// Hint: This will not disturb any running operations!
  /// </summary>
  /// <param name="subscriptionId">ID of subscription</param>
  /// <returns>Just a simple task</returns>
  public Task CancelSubscription(Guid subscriptionId);


  /// <summary>
  /// 'Fire and forget' a message
  /// </summary>
  /// <param name="request">Message to be sent</param>
  /// <returns>A simple Task</returns>
  public Task Publish(IRequest request);


  /// <summary>
  /// Fire a request and get (possibly) more than one response
  /// </summary>
  /// <param name="request">Request to be fired</param>
  /// <returns>Array of responses</returns>
  public Task<IResponse[]> GetResponses(IRequest request);


  /// <summary>
  /// Fire a request and get (possibly) more than one response
  /// </summary>
  /// <param name="request">Request to be fired</param>
  /// <returns>Array of responses</returns>
  public async Task<TResponse[]> GetResponses<TResponse>(IRequest request)
    where TResponse : IResponse
    => (await GetResponses(request)).OfType<TResponse>().ToArray();


  /// <summary>
  /// Fire a request and get (possibly) one response
  /// Hint: If more than one response found, just the 1st one will be returned. It is not predictable which one will be the first!
  /// </summary>
  /// <param name="request">Request to be fired</param>
  /// <returns>One response or default if no response was delivered</returns>
  public async Task<IResponse> GetResponse(IRequest request)
    => (await GetResponses(request)).FirstOrDefault();


  /// <summary>
  /// Fire a request and get (possibly) one response
  /// Hint: If more than one response found, just the 1st one will be returned. It is not predictable which one will be the first!
  /// </summary>
  /// <param name="request">Request to be fired</param>
  /// <returns>One response or default if no response was delivered</returns>
  public async Task<TResponse> GetResponse<TResponse>(IRequest request)
    where TResponse : IResponse
    => (await GetResponses(request)).OfType<TResponse>().FirstOrDefault();
}
