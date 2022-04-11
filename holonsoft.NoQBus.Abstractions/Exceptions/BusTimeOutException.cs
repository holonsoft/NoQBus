using System.Runtime.Serialization;

namespace holonsoft.NoQBus.Abstractions.Exceptions;

/// <summary>
/// Internal bus exception to make sure that we never wait endless on a call
/// </summary>
public class BusTimeOutException : Exception
{
  public BusTimeOutException()
  {
  }

  public BusTimeOutException(string message) : base(message)
  {
  }

  public BusTimeOutException(string message, Exception innerException) : base(message, innerException)
  {
  }

  public BusTimeOutException(SerializationInfo info, StreamingContext context) : base(info, context)
  {
  }
}
