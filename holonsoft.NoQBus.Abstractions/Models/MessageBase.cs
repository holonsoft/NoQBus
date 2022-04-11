using holonsoft.NoQBus.Abstractions.Contracts;

namespace holonsoft.NoQBus.Abstractions.Models;

/// <summary>
/// Base structure for all messages
/// </summary>
public abstract record MessageBase : IMessage
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

  /// <summary>
  /// process global sender ID
  /// </summary>
  private static readonly string _globalSenderId = Guid.NewGuid().ToString();

  /// <summary>
  /// Standard constructor
  /// </summary>
  protected MessageBase()
  {
    Culture = System.Globalization.CultureInfo.CurrentUICulture.Name; //e.g. en-US, de-DE
    SenderId = _globalSenderId;
    MessageId = Guid.NewGuid();
    SessionId = "";
    CreationTimeStamp = DateTime.UtcNow;
  }

  /// <summary>
  /// Constructor that partial clones some data from a given message.
  /// Timestamp and message ID will not be cloned but set with new values
  /// </summary>
  /// <param name="cloneFromMessageButNewTimestampAndGuid"></param>
  protected MessageBase(IMessage cloneFromMessageButNewTimestampAndGuid)
  {
    Culture = cloneFromMessageButNewTimestampAndGuid.Culture;
    SenderId = cloneFromMessageButNewTimestampAndGuid.SenderId;
    SessionId = cloneFromMessageButNewTimestampAndGuid.SessionId;

    CreationTimeStamp = DateTime.UtcNow;
    MessageId = Guid.NewGuid();
  }
}
