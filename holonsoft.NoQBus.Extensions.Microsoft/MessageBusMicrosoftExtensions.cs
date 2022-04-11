using holonsoft.NoQBus.Abstractions.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace holonsoft.NoQBus.Extensions.Microsoft;
public static class MessageBusMicrosoftExtensions
{
  public static void AddNoQMessageBus(this IServiceCollection serviceCollection)
  {
    serviceCollection.TryAddSingleton<MessageBus>();
    serviceCollection.TryAddSingleton<IMessageBus>(x => x.GetService<MessageBus>());
    serviceCollection.TryAddSingleton<IMessageBusConfig>(x => x.GetService<MessageBus>());
    serviceCollection.TryAddSingleton<IRemoteMessageBus>(x => x.GetService<MessageBus>());
    serviceCollection.TryAddSingleton<IMessageBusFiltering>(x => x.GetService<MessageBus>());
  }

  public static Task StartLocalNoQMessageBus(this IServiceProvider serviceProvider,
                                             CancellationToken cancellationToken = default)
    => serviceProvider.GetRequiredService<IMessageBusConfig>().StartLocalNoQMessageBus(cancellationToken);
}
