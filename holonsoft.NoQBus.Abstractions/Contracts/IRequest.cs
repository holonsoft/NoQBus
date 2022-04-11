namespace holonsoft.NoQBus.Abstractions.Contracts;

/// <summary>
/// Base interface for requests
/// </summary>
public interface IRequest : IMessage
{
}


/// <summary>
/// Base interface for requests and corresponding responses
/// </summary>
public interface IRequest<TResponse> : IRequest
  where TResponse : IResponse
{

}
