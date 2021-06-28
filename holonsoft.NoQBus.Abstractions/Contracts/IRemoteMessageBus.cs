using System.Threading.Tasks;

namespace holonsoft.NoQBus.Abstractions.Contracts
{
   public interface IRemoteMessageBus
   {
      public Task<IResponse[]> GetResponsesForRemotedRequest(IRequest request);
   }
}
