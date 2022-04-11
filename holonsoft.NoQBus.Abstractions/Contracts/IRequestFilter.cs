namespace holonsoft.NoQBus.Abstractions.Contracts;

/// <summary>
/// Base interface for filtering request messages
/// </summary>
public interface IRequestFilter
{
  /// <summary>
  /// Filter a message
  /// </summary>
  /// <param name="request">message to be inspected</param>
  /// <returns>true, if message can be processed, otherwise false</returns>
  public Task<bool> Filter(IRequest request);
}

/// <summary>
/// Base interface for filtering request messages
/// </summary>
/// <typeparam name="TRequest"></typeparam>
public interface IRequestFilter<in TRequest>
  where TRequest : IRequest
{
  /// <summary>
  /// Filter a message
  /// </summary>
  /// <param name="request">message to be inspected</param>
  /// <returns>true, if message can be processed, otherwise false</returns>
  public Task<bool> Filter(TRequest request);
}
