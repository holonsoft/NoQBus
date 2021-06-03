using System;
using System.Runtime.Serialization;

namespace holonsoft.NoQBus
{
   public class NobodyListeningException : Exception
   {
      public NobodyListeningException()
      {
      }

      public NobodyListeningException(string message) : base(message)
      {
      }

      public NobodyListeningException(string message, Exception innerException) : base(message, innerException)
      {
      }

      protected NobodyListeningException(SerializationInfo info, StreamingContext context) : base(info, context)
      {
      }
   }
}
