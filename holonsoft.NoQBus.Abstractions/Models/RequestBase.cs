using holonsoft.NoQBus.Abstractions.Contracts;

namespace holonsoft.NoQBus.Abstractions.Models;

/// <summary>
/// Base record structure for a request
/// </summary>
public abstract record RequestBase : MessageBase, IRequest
{
  /// <summary>
  /// Standard constructor
  /// </summary>
  protected RequestBase()
    : base()
  {
  }

  /// <summary>
  /// Constructor that partial clones some data from a given message.
  /// Timestamp and message ID will not be cloned but set with new values
  /// </summary>
  /// <param name="cloneFromMessageButNewTimestampAndGuid"></param>
  protected RequestBase(IMessage cloneFromMessageButNewTimestampAndGuid)
    : base(cloneFromMessageButNewTimestampAndGuid)
  {
  }
}


/// <summary>
/// Base record structure for a request
/// </summary>
public abstract record RequestBase<TResponse> : RequestBase, IRequest<TResponse>
  where TResponse : IResponse
{
  /// <summary>
  /// Standard constructor
  /// </summary>
  protected RequestBase()
    : base()
  {
  }


  /// <summary>
  /// Constructor that partial clones some data from a given message.
  /// Timestamp and message ID will not be cloned but set with new values
  /// </summary>
  /// <param name="cloneFromMessageButNewTimestampAndGuid"></param>
  protected RequestBase(IMessage cloneFromMessageButNewTimestampAndGuid)
    : base(cloneFromMessageButNewTimestampAndGuid)
  {
  }
}
