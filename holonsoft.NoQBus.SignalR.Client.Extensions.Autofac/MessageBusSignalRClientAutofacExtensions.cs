using Autofac;
using holonsoft.NoQBus.Abstractions.Contracts;
using holonsoft.NoQBus.Serialization;

namespace holonsoft.NoQBus.SignalR.Client.Extensions.Autofac;

public static class MessageBusSignalRClientAutofacExtensions
{
  public static void AddNoQSignalRClient(this ContainerBuilder containerBuilder)
  {
    containerBuilder
      .RegisterType<MessageBusSignalRClient>()
      .AsSelf()
      .As<IMessageBusSignalRClientConfig>()
      .As<IMessageBusSink>()
      .SingleInstance();

    containerBuilder
      .RegisterType<MessageSerializer>()
      .As<IMessageSerializer>()
      .SingleInstance();
  }

  public static Task StartNoQSignalRClient(this ILifetimeScope lifetimeScope,
                                           Action<IMessageBusSignalRClientConfig> configure = null,
                                           CancellationToken cancellationToken = default)
    => lifetimeScope.Resolve<IMessageBusConfig>().StartNoQSignalRClient(configure, cancellationToken);

  public static Task StartNoQSignalRClient(this ILifetimeScope lifetimeScope,
                                           string url,
                                           CancellationToken cancellationToken = default)
    => lifetimeScope.Resolve<IMessageBusConfig>().StartNoQSignalRClient(url, cancellationToken);
}
