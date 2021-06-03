using System.Threading.Tasks;

namespace holonsoft.NoQBus
{
   public interface IMessageBusSinkTransport
   {
      public Task<SinkTransportDataResponse> TransportToEndpoint(SinkTransportDataRequest request);
   }
}
