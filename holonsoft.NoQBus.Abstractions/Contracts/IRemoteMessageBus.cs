using System.Threading.Tasks;

namespace holonsoft.NoQBus
{
   public interface IRemoteMessageBus
   {
      public Task<IResponse[]> GetResponsesForRemotedRequest(IRequest request);
   }
}
