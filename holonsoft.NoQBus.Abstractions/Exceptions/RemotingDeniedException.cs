using System;
using System.Runtime.Serialization;

namespace holonsoft.NoQBus
{
   public class RemotingDeniedException : Exception
   {
      public RemotingDeniedException()
      {
      }

      public RemotingDeniedException(string message) : base(message)
      {
      }

      public RemotingDeniedException(string message, Exception innerException) : base(message, innerException)
      {
      }

      protected RemotingDeniedException(SerializationInfo info, StreamingContext context) : base(info, context)
      {
      }
   }
}
