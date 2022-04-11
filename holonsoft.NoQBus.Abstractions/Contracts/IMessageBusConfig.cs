namespace holonsoft.NoQBus.Abstractions.Contracts;

/// <summary>
/// Provides a startpoint for messagebus configuration
/// </summary>
public interface IMessageBusConfig
{
  /// <summary>
  /// Startpoint for messagebus configuration in a fluent manner
  /// </summary>
  /// <returns><see cref="IMessageBusConfigure"/></returns>
  public IMessageBusConfigure Configure();
}
