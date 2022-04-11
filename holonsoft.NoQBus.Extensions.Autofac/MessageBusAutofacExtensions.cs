using Autofac;
using holonsoft.NoQBus.Abstractions.Contracts;

namespace holonsoft.NoQBus.Extensions.Autofac;
public static class MessageBusAutofacExtensions
{
  public static void AddNoQMessageBus(this ContainerBuilder containerBuilder) => containerBuilder
       .RegisterType<MessageBus>()
       .As<IMessageBus>()
       .As<IMessageBusConfig>()
       .As<IRemoteMessageBus>()
       .As<IMessageBusFiltering>()
       .SingleInstance();
  public static Task StartLocalNoQMessageBus(this ILifetimeScope lifetimeScope,
                                             CancellationToken cancellationToken = default)
    => lifetimeScope.Resolve<IMessageBusConfig>().StartLocalNoQMessageBus(cancellationToken);
}
