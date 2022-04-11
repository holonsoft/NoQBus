using holonsoft.NoQBus.Abstractions.Exceptions;

namespace holonsoft.NoQBus.Abstractions.Contracts;

/// <summary>
/// Entrypoint for configuring the messagebus 
/// </summary>
public interface IMessageBusConfigure
{
  /// <summary>
  /// Defines a timespan the bus waits for answers on a request. 
  /// </summary>
  /// <param name="timeOutTimeSpan">timespan to wait</param>
  /// <returns><see cref="IMessageBusConfigure"/></returns>
  public IMessageBusConfigure SetTimeoutTimeSpan(TimeSpan timeOutTimeSpan);


  /// <summary>
  /// If this flag is set the bus throws <see cref="NobodyListeningException"/>
  /// Setting together with <see cref="DoNotThrowIfNoReceiverSubscribed"/> has undefined behaviour
  /// </summary>
  /// <returns><see cref="IMessageBusConfigure"/></returns>
  public IMessageBusConfigure ThrowIfNoReceiverSubscribed();


  /// <summary>
  /// If this flag is set the bus ignores messages if no subscription is provided
  /// Setting together with <see cref="ThrowIfNoReceiverSubscribed"/> has undefined behaviour
  /// </summary>
  /// <returns><see cref="IMessageBusConfigure"/></returns>
  public IMessageBusConfigure DoNotThrowIfNoReceiverSubscribed();


  /// <summary>
  /// Provides an action for configuring the sink (= remoting)
  /// </summary>
  /// <param name="sinkConfig"></param>
  /// <returns><see cref="IMessageBusConfigure"/></returns>
  public IMessageBusConfigure ConfigureSink(Action<IMessageBusSink> sinkConfig);


  /// <summary>
  /// Finish the configuration and starts the bus
  /// </summary>
  /// <param name="cancellationToken"></param>
  /// <returns>A simple TASK</returns>
  public Task StartAsync(CancellationToken cancellationToken = default);
}
