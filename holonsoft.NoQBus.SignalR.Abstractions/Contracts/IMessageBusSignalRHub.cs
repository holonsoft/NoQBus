using System.Threading.Tasks;

namespace holonsoft.NoQBus.SignalR
{
   public interface IMessageBusSignalRHub
   {
      public Task<SinkTransportDataResponse> ProcessMessage(SinkTransportDataRequest request);
      public Task<bool> ReceiveResponse(SinkTransportDataResponse response);
   }
}
