using System.Threading.Tasks;

namespace holonsoft.NoQBus
{
   public interface IMessageBusSink
   {
      public Task<IResponse[]> GetResponses(IRequest request);

      public Task<SinkTransportDataResponse> GetResponsesForRemotedRequest(SinkTransportDataRequest request);
   }
}
