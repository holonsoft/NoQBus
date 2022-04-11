using holonsoft.NoQBus.Abstractions.Contracts;
using holonsoft.NoQBus.Abstractions.Exceptions;
using holonsoft.NoQBus.Remoting;
using holonsoft.NoQBus.Remoting.Models;
using holonsoft.NoQBus.SignalR.Abstractions;
using holonsoft.NoQBus.SignalR.Abstractions.Contracts;
using Microsoft.AspNetCore.SignalR.Client;
using SignalR.Strong;

namespace holonsoft.NoQBus.SignalR.Client;

public partial class MessageBusSignalRClient
  : MessageBusSinkBase, IMessageBusSignalRClient,
    IAsyncDisposable
{
  private HubConnection _connection;
  private SpokeRegistration _connectionSpoke;
  private IMessageBusSignalRHub _typedHub;

  public MessageBusSignalRClient(IMessageSerializer messageSerializer) : base(messageSerializer)
  {
  }

  public string Url { get; private set; } = MessageBusSignalRConstants.DefaultUrl;
  public TimeSpan RetryDelay { get; private set; } = MessageBusSignalRConstants.DefaultRetryDelay;
  public int InitialRetryCount { get; private set; } = MessageBusSignalRConstants.DefaultInitialRetryCount;

  public override Task StartAsync(CancellationToken cancellationToken = default)
  {
    _connection = new HubConnectionBuilder()
                     .WithUrl(Url)
                     .WithAutomaticReconnect(new RetryPolicy(RetryDelay))
                     .Build();

    _connectionSpoke = _connection.RegisterSpoke<IMessageBusSignalRClient>(this);

    _typedHub = _connection.AsDynamicHub<IMessageBusSignalRHub>();

    return ConnectWithRetryAsync();

    //RetryPolicy is not used on first connection
    async Task ConnectWithRetryAsync()
    {
      while (true)
      {
        try
        {
          await _connection.StartAsync(cancellationToken);
          return;
        }
        catch when (cancellationToken.IsCancellationRequested)
        {
          return;
        }
        catch
        {
          if (--InitialRetryCount <= 0)
          {
            throw new BusTimeOutException($"Connection to SignalR Server using Url '{Url}' failed!");
          }

          await Task.Delay(RetryDelay, cancellationToken);
          if (cancellationToken.IsCancellationRequested)
          {
            return;
          }
        }
      }
    }
  }

  public override Task<SinkTransportDataResponse> TransportToEndpoint(SinkTransportDataRequest request)
    => _typedHub.ProcessMessage(request);

  async Task IMessageBusSignalRClient.ProcessMessage(SinkTransportDataRequest request)
  {
    var response = await GetResponsesForRemoteRequest(request);
    await _typedHub.ReceiveResponse(response);
  }

  public async ValueTask DisposeAsync()
  {
    GC.SuppressFinalize(this);
    _connectionSpoke.Dispose();
    try
    {
      //when in an container it could be that this was already disposed
      await _connection.StopAsync();
      await _connection.DisposeAsync();

    }
    catch (Exception) { }
  }

  private class RetryPolicy : IRetryPolicy
  {
    private readonly TimeSpan _retryDelay;

    public RetryPolicy(TimeSpan retryDelay)
      => this._retryDelay = retryDelay;

    public TimeSpan? NextRetryDelay(RetryContext retryContext) => _retryDelay;
  }
}
