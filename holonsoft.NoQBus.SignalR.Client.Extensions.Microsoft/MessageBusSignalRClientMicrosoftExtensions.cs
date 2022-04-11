using holonsoft.NoQBus.Abstractions.Contracts;
using holonsoft.NoQBus.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace holonsoft.NoQBus.SignalR.Client.Extensions.Microsoft;

public static class MessageBusSignalRClientMicrosoftExtensions
{
  public static void AddNoQSignalRClient(this IServiceCollection serviceCollection)
  {
    serviceCollection.TryAddSingleton<MessageBusSignalRClient>();
    serviceCollection.TryAddSingleton<IMessageBusSignalRClientConfig>(x => x.GetService<MessageBusSignalRClient>());
    serviceCollection.TryAddSingleton<IMessageBusSink>(x => x.GetService<MessageBusSignalRClient>());
    serviceCollection.TryAddSingleton<IMessageSerializer, MessageSerializer>();
  }

  public static Task StartNoQSignalRClient(this IServiceProvider serviceProvider,
                                           Action<IMessageBusSignalRClientConfig> configure = null,
                                           CancellationToken cancellationToken = default)
    => serviceProvider.GetRequiredService<IMessageBusConfig>().StartNoQSignalRClient(configure, cancellationToken);

  public static Task StartNoQSignalRClient(this IServiceProvider serviceProvider,
                                           string url,
                                           CancellationToken cancellationToken = default)
    => serviceProvider.GetRequiredService<IMessageBusConfig>().StartNoQSignalRClient(url, cancellationToken);
}
