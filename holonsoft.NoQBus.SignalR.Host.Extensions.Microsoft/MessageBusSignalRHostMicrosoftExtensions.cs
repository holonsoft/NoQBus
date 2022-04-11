using holonsoft.NoQBus.Abstractions.Contracts;
using holonsoft.NoQBus.Serialization;
using holonsoft.NoQBus.SignalR.Abstractions.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace holonsoft.NoQBus.SignalR.Host.Extensions.Microsoft;

public static class MessageBusSignalRHostMicrosoftExtensions
{
  public static void AddNoQSignalRHost(this IServiceCollection serviceCollection)
  {
    serviceCollection.TryAddSingleton<MessageBusSignalRHost>();
    serviceCollection.TryAddSingleton<IMessageBusSignalRHostConfig>(x => x.GetService<MessageBusSignalRHost>());
    serviceCollection.TryAddSingleton<IMessageBusSink>(x => x.GetService<MessageBusSignalRHost>());
    serviceCollection.TryAddSingleton<MessageBusSignalRHubStateStore>();
    serviceCollection.TryAddSingleton<IMessageSerializer, MessageSerializer>();
  }

  public static async Task StartNoQSignalRHostOnExistingAspNetCoreHost(
    this IHost host,
    Action<IMessageBusSignalRHostConfig> configure = null,
    CancellationToken cancellationToken = default)
  {
    var messageBusSignalRHost = host.Services.GetRequiredService<MessageBusSignalRHost>();
    messageBusSignalRHost.ExistingAspNetCoreHost = true;
    var messageBusConfig = host.Services.GetRequiredService<IMessageBusConfig>();

    await messageBusConfig.StartNoQSignalRHost(configure, cancellationToken);
  }

  public static Task StartNoQSignalRHost(this IServiceProvider serviceProvider,
                                         Action<IMessageBusSignalRHostConfig> configure = default,
                                         CancellationToken cancellationToken = default)
    => serviceProvider.GetRequiredService<IMessageBusConfig>().StartNoQSignalRHost(configure, cancellationToken);

  public static void UseNoQSignalRHost(this IApplicationBuilder applicationBuilder)
  {
    var host = applicationBuilder.ApplicationServices.GetRequiredService<MessageBusSignalRHost>();
    var context = applicationBuilder.ApplicationServices.GetRequiredService<IHubContext<MessageBusSignalRHub, IMessageBusSignalRClient>>();
    host.SetHubContext(context);
  }
}
