using System.Threading.Tasks;

namespace holonsoft.NoQBus.Abstractions.Contracts
{
   public interface IRespondToRequest
   {
      public Task<IResponse> Respond(IRequest request);
   }

   public interface IRespondToRequest<in TRequest, TResponse> where TRequest : IRequest
                                                           where TResponse : IResponse
   {
      public Task<TResponse> Respond(TRequest request);
   }
}
