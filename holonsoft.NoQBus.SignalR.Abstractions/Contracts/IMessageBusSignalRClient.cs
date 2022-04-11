using holonsoft.NoQBus.Remoting.Models;

namespace holonsoft.NoQBus.SignalR.Abstractions.Contracts
{
  public interface IMessageBusSignalRClient
  {
    public Task ProcessMessage(SinkTransportDataRequest request);
  }
}
