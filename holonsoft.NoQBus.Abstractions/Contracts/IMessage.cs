namespace holonsoft.NoQBus.Abstractions.Contracts;

/// <summary>
/// A standard message provides some basics, defined here.
/// </summary>
public interface IMessage
{
  /// <summary>
  /// Provides sender-side culture infos 
  /// </summary>
  public string Culture { get; init; }


  /// <summary>
  /// An arbitrary sender ID 
  /// </summary>
  public string SenderId { get; init; }


  /// <summary>
  /// Every message has an unique id
  /// </summary>
  public Guid MessageId { get; init; }


  /// <summary>
  /// An arbitrary session ID
  /// </summary>
  public string SessionId { get; init; }


  /// <summary>
  /// Creation timestamp of message
  /// </summary>
  public DateTime CreationTimeStamp { get; init; }
}
