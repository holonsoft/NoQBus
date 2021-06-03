using System.Threading.Tasks;

namespace holonsoft.NoQBus
{
   public interface IConsumer
   {
      public Task Consume(IRequest request);
   }

   public interface IConsumer<TRequest> where TRequest : IRequest
   {
      public Task Consume(TRequest request);
   }
}
