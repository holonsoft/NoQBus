using holonsoft.NoQBus.Abstractions.Models;
using System.Threading.Tasks;

namespace holonsoft.NoQBus.SignalR.Abstractions.Contracts
{
   public interface IMessageBusSignalRClient
   {
      public Task ProcessMessage(SinkTransportDataRequest request);
   }
}
