using System.Threading.Tasks;

namespace holonsoft.NoQBus.SignalR
{
   public interface IMessageBusSignalRClient
   {
      public Task ProcessMessage(SinkTransportDataRequest request);
   }
}
