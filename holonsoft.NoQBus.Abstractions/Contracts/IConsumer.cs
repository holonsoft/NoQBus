namespace holonsoft.NoQBus.Abstractions.Contracts;

/// <summary>
/// Provide a consumer implementation for a received request
/// </summary>
public interface IConsumer
{
  /// <summary>
  /// Consume the request
  /// </summary>
  /// <param name="request">Request to be handled</param>
  /// <returns>A simple Task</returns>
  public Task Consume(IRequest request);
}

/// <summary>
/// Provide a consumer implementation for a received request
/// </summary>
public interface IConsumer<in TRequest>
  where TRequest : IRequest
{
  /// <summary>
  /// Consume the request
  /// </summary>
  /// <param name="request">Request to be handled</param>
  /// <returns>A simple Task</returns>
  public Task Consume(TRequest request);
}
