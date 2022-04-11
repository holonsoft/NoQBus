using holonsoft.NoQBus.Abstractions.Contracts;

namespace holonsoft.NoQBus;
public static class MessageBusConfigurationHelper
{
  public static Task StartLocalNoQMessageBus(this IMessageBusConfig config,
                                             CancellationToken cancellationToken = default) => config
        .Configure()
        .DoNotThrowIfNoReceiverSubscribed()
        .StartAsync(cancellationToken);
}
