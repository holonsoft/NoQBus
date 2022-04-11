using holonsoft.NoQBus.Abstractions.Contracts;
using holonsoft.NoQBus.Abstractions.Models;
using holonsoft.NoQBus.SignalR.Abstractions.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace holonsoft.NoQBus.SignalR.Host;

public class MessageBusSignalRHub : Hub<IMessageBusSignalRClient>, IMessageBusSignalRHub
{
  private readonly IMessageBusSink _host;
  private readonly MessageBusSignalRHubStateStore _stateStore;

  public MessageBusSignalRHub(MessageBusSignalRHost host, MessageBusSignalRHubStateStore stateStore)
  {
    _host = host;
    _stateStore = stateStore;
  }

  public override Task OnConnectedAsync()
  {
    _stateStore.ClientConnected(Context.ConnectionId);
    return base.OnConnectedAsync();
  }

  public override Task OnDisconnectedAsync(Exception exception)
  {
    _stateStore.ClientDisconnected(Context.ConnectionId);
    return base.OnDisconnectedAsync(exception);
  }

  public Task<SinkTransportDataResponse> ProcessMessage(SinkTransportDataRequest request)
      => _host.GetResponsesForRemoteRequest(request);

  public Task<bool> ReceiveResponse(SinkTransportDataResponse response)
  {
    _stateStore.ReceivedResponse(Context.ConnectionId, response);
    return Task.FromResult(true);
  }
}
