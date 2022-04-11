namespace holonsoft.NoQBus.Abstractions.Contracts;

/// <summary>
/// Base interface for request/response pattern
/// </summary>
public interface IRespondToRequest
{
  /// <summary>
  /// Respond to a request
  /// </summary>
  /// <param name="request">request to be fired</param>
  /// <returns>a task within a <see cref="IResponse"/></returns>
  public Task<IResponse> Respond(IRequest request);
}


/// <summary>
/// Base interface for request/response pattern
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public interface IRespondToRequest<in TRequest, TResponse>
  where TRequest : IRequest
  where TResponse : IResponse
{
  /// <summary>
  /// Respond to a request
  /// </summary>
  /// <param name="request">request to be fired</param>
  /// <returns>a task within a response</returns>
  public Task<TResponse> Respond(TRequest request);
}
