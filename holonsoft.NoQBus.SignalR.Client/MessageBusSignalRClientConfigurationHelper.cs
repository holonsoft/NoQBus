using holonsoft.NoQBus.Abstractions.Contracts;

namespace holonsoft.NoQBus.SignalR.Client;

public static class MessageBusSignalRClientConfigurationHelper
{

  public static Task StartNoQSignalRClient(this IMessageBusConfig config,
                                           Action<IMessageBusSignalRClientConfig> configure = null,
                                           CancellationToken cancellationToken = default) => config
        .Configure()
        .ThrowIfNoReceiverSubscribed()
        .ConfigureSink(x => configure?.Invoke((IMessageBusSignalRClientConfig) x))
        .StartAsync(cancellationToken);

  public static Task StartNoQSignalRClient(this IMessageBusConfig config,
                                           string url,
                                           CancellationToken cancellationToken = default)
    => StartNoQSignalRClient(config, x => x.UseUrl(url), cancellationToken);
}
