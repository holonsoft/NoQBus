using System.Threading.Tasks;

namespace holonsoft.NoQBus.Abstractions.Contracts
{
   public interface IConsumer
   {
      public Task Consume(IRequest request);
   }

   public interface IConsumer<in TRequest> where TRequest : IRequest
   {
      public Task Consume(TRequest request);
   }
}
