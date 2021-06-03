using System;

namespace holonsoft.NoQBus
{
   public interface IResponse : IMessage
   {
      public Guid CorrospondingRequestMessageId { get; }
   }

   public interface IResponse<TRequest> : IResponse where TRequest : IRequest
   {

   }
}
