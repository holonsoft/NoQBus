namespace holonsoft.NoQBus.Abstractions.Contracts
{
   public interface IRequest : IMessage
   {
   }

   public interface IRequest<TResponse> : IRequest 
	   where TResponse : IResponse
   {

   }
}
