using Autofac;
using holonsoft.NoQBus.Abstractions.Contracts;
using holonsoft.NoQBus.Serialization;

namespace holonsoft.NoQBus.SignalR.Host.Extensions.Autofac;

public static class MessageBusSignalRHostAutofacExtensions
{
  public static void AddNoQSignalRHost(this ContainerBuilder containerBuilder)
  {
    containerBuilder
       .RegisterType<MessageBusSignalRHost>()
       .AsSelf()
       .As<IMessageBusSink>()
       .As<IMessageBusSignalRHostConfig>()
       .SingleInstance();

    containerBuilder
       .RegisterType<MessageBusSignalRHubStateStore>()
       .AsSelf()
       .SingleInstance();

    containerBuilder
      .RegisterType<MessageSerializer>()
      .As<IMessageSerializer>()
      .SingleInstance();
  }

  public static Task StartNoQSignalRHost(this ILifetimeScope lifetimeScope,
                                         Action<IMessageBusSignalRHostConfig> configure = default,
                                         CancellationToken cancellationToken = default)
    => lifetimeScope.Resolve<IMessageBusConfig>().StartNoQSignalRHost(configure, cancellationToken);
}
