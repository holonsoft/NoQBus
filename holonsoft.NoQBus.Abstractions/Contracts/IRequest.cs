namespace holonsoft.NoQBus
{
   public interface IRequest : IMessage
   {
   }

   public interface IRequest<TResponse> : IRequest where TResponse : IResponse
   {

   }
}
