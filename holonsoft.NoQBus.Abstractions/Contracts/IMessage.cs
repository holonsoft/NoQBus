using System;

namespace holonsoft.NoQBus
{
   public interface IMessage
   {
      public string Culture { get; }
      public string SenderId { get; }
      public Guid MessageId { get; }
      public string AuthToken { get; }
   }
}
