using System.Threading.Tasks;

namespace holonsoft.NoQBus
{
   public interface IRespondToRequest
   {
      public Task<IResponse> Respond(IRequest request);
   }

   public interface IRespondToRequest<TRequest, TResponse> where TRequest : IRequest
                                                           where TResponse : IResponse
   {
      public Task<TResponse> Respond(TRequest request);
   }
}
