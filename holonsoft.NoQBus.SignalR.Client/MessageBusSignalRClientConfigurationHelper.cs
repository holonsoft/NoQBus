using Autofac;
using holonsoft.NoQBus.Abstractions.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace holonsoft.NoQBus.SignalR.Client;

public static class MessageBusSignalRClientConfigurationHelper
{
  public static void AddNoQSignalRClient(this ContainerBuilder containerBuilder) => containerBuilder
       .RegisterType<MessageBusSignalRClient>()
       .AsSelf()
       .As<IMessageBusSignalRClientConfig>()
       .As<IMessageBusSink>()
       .SingleInstance();

  public static void AddNoQSignalRClient(this IServiceCollection serviceCollection)
  {
    serviceCollection.TryAddSingleton<MessageBusSignalRClient>();
    serviceCollection.TryAddSingleton<IMessageBusSignalRClientConfig>(x => x.GetService<MessageBusSignalRClient>());
    serviceCollection.TryAddSingleton<IMessageBusSink>(x => x.GetService<MessageBusSignalRClient>());
  }

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

  public static Task StartNoQSignalRClient(this ILifetimeScope lifetimeScope,
                                           Action<IMessageBusSignalRClientConfig> configure = null,
                                           CancellationToken cancellationToken = default)
    => lifetimeScope.Resolve<IMessageBusConfig>().StartNoQSignalRClient(configure, cancellationToken);

  public static Task StartNoQSignalRClient(this ILifetimeScope lifetimeScope,
                                           string url,
                                           CancellationToken cancellationToken = default)
    => lifetimeScope.Resolve<IMessageBusConfig>().StartNoQSignalRClient(url, cancellationToken);

  public static Task StartNoQSignalRClient(this ServiceProvider serviceProvider,
                                           Action<IMessageBusSignalRClientConfig> configure = null,
                                           CancellationToken cancellationToken = default)
    => serviceProvider.GetRequiredService<IMessageBusConfig>().StartNoQSignalRClient(configure, cancellationToken);

  public static Task StartNoQSignalRClient(this ServiceProvider serviceProvider,
                                           string url,
                                           CancellationToken cancellationToken = default)
    => serviceProvider.GetRequiredService<IMessageBusConfig>().StartNoQSignalRClient(url, cancellationToken);
}
