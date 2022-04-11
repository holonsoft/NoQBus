using holonsoft.NoQBus.Abstractions.Contracts;
using holonsoft.NoQBus.Remoting;
using holonsoft.NoQBus.Remoting.Models;
using holonsoft.NoQBus.SignalR.Abstractions;
using holonsoft.NoQBus.SignalR.Abstractions.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace holonsoft.NoQBus.SignalR.Host;

public partial class MessageBusSignalRHost : MessageBusSinkBase
{
  private readonly MessageBusSignalRHubStateStore _stateStore;

  private IHubContext<MessageBusSignalRHub, IMessageBusSignalRClient> _hubContext;
  public void SetHubContext(IHubContext<MessageBusSignalRHub, IMessageBusSignalRClient> hubContext) => _hubContext = _hubContext == null ? hubContext : throw new NotSupportedException($"HubContext for the {nameof(MessageBusSignalRHost)} is already set!");
  protected IHubContext<MessageBusSignalRHub, IMessageBusSignalRClient> EnsureHubContext()
     => _hubContext ?? throw new NotSupportedException($"HubContext for the {nameof(MessageBusSignalRHost)} is not set!");

  public string Url { get; private set; } = MessageBusSignalRConstants.DefaultUrl;
  public bool ExistingAspNetCoreHost { get; set; }

  public MessageBusSignalRHost(MessageBusSignalRHubStateStore stateStore, IMessageSerializer messageSerializer) : base(messageSerializer)
    => _stateStore = stateStore;

  public override async Task<SinkTransportDataResponse> TransportToEndpoint(SinkTransportDataRequest request)
  {
    var hubContext = EnsureHubContext();

    var connections = _stateStore.GetConnections();

    if (connections.Length > 0)
    {
      var result = _stateStore.AwaitResultForRequest(request, connections);

      List<(string Connection, Task Task)> requestsToClient = new();
      foreach (var connection in connections)
      {
        try
        {
          var client = hubContext.Clients.Client(connection);

          requestsToClient.Add((connection, client.ProcessMessage(request)));
        }
        catch (Exception)
        {
          _stateStore.ClientDisconnected(connection);
        }
      }

      foreach (var requestToClient in requestsToClient)
      {
        try
        {
          await requestToClient.Task;
        }
        catch (Exception)
        {
          _stateStore.ClientDisconnected(requestToClient.Connection);
        }
      }

      return await result;
    }

    return new SinkTransportDataResponse(request);
  }

  public override async Task StartAsync(CancellationToken cancellationToken = default)
  {
    if (!ExistingAspNetCoreHost)
    {
      UriBuilder uriBuilder = new(Url);
      uriBuilder.Path = "";
      uriBuilder.Query = "";
      uriBuilder.Fragment = "";

      var host = new HostBuilder()
                 .ConfigureWebHostDefaults(webBuilder =>
                 {
                   webBuilder.UseUrls(uriBuilder.ToString());

                   webBuilder.ConfigureServices(services =>
                   {
                     services.AddSingleton(_stateStore);
                     services.AddSingleton(this);
                     services.AddSignalR(o =>
                     {
                       o.EnableDetailedErrors = true;
                       o.MaximumParallelInvocationsPerClient = int.MaxValue;
                       o.MaximumReceiveMessageSize = int.MaxValue;
                     });
                   });

                   webBuilder.Configure(app =>
                   {
                     var host = app.ApplicationServices.GetRequiredService<MessageBusSignalRHost>();
                     var context = app.ApplicationServices.GetRequiredService<IHubContext<MessageBusSignalRHub, IMessageBusSignalRClient>>();
                     host.SetHubContext(context);

                     app.UseRouting();

                     app.UseEndpoints(endpoints =>
                     {
                       endpoints.MapNoQSignalRHost(new UriBuilder(Url).Path);
                     });
                   });
                 })
                 .Build();

      await host.StartAsync(cancellationToken);

      async Task WaitForShutdownAndDispose()
      {
        await host.WaitForShutdownAsync(cancellationToken);
        await host.StopAsync(CancellationToken.None);
        host.Dispose();
      }

      _ = WaitForShutdownAndDispose();
    }
  }
}
