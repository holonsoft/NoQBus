namespace holonsoft.NoQBus.Abstractions.Contracts;

/// <summary>
/// Base interface for filtering response messages
/// </summary>
public interface IResponseFilter
{
  /// <summary>
  /// Process a list of responses
  /// </summary>
  /// <param name="responses">An enumerable list of <see cref="IResponse"/></param>
  /// <returns>List of responses that can be processed further</returns>
  public Task<IEnumerable<IResponse>> Filter(IEnumerable<IResponse> responses);
}


/// <summary>
/// Base interface for filtering response messages
/// </summary>
public interface IResponseFilter<TResponse>
  where TResponse : IResponse
{
  /// <summary>
  /// Process a list of responses
  /// </summary>
  /// <param name="responses">An enumerable list of <see cref="IResponse"/></param>
  /// <returns>List of responses that can be processed further</returns>
  public Task<IEnumerable<TResponse>> Filter(IEnumerable<TResponse> responses);
}
