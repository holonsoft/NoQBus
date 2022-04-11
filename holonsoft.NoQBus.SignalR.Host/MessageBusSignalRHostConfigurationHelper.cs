using holonsoft.NoQBus.Abstractions.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace holonsoft.NoQBus.SignalR.Host;

public static class MessageBusSignalRHostConfigurationHelper
{
  public static void MapNoQSignalRHost(this IEndpointRouteBuilder endpointRouteBuilder, string path)
    => endpointRouteBuilder.MapHub<MessageBusSignalRHub>(path);

  public static Task StartNoQSignalRHost(this IMessageBusConfig config,
                                        Action<IMessageBusSignalRHostConfig> configure = default,
                                        CancellationToken cancellationToken = default) => config
        .Configure()
        .DoNotThrowIfNoReceiverSubscribed()
        .ConfigureSink(x => configure?.Invoke((IMessageBusSignalRHostConfig) x))
        .StartAsync(cancellationToken);
}
