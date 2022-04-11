using holonsoft.NoQBus.Abstractions.Models;

namespace holonsoft.NoQBus.SignalR.Abstractions.Contracts
{
  public interface IMessageBusSignalRClient
  {
    public Task ProcessMessage(SinkTransportDataRequest request);
  }
}
