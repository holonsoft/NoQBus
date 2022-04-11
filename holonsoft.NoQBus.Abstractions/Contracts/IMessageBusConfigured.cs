namespace holonsoft.NoQBus.Abstractions.Contracts;

/// <summary>
/// Get info whether configuration has applied an messagebus is up-and-running
/// </summary>
public interface IMessageBusConfigured
{
  /// <summary>
  /// Get info whether configuration has applied an messagebus is up-and-running
  /// </summary>
  bool IsConfigured { get; }
}

