using System;
using System.Runtime.Serialization;

namespace holonsoft.NoQBus.Abstractions.Exceptions
{
	/// <summary>
	/// Thrown if no-one listens locally to a message request
	/// Hint: must be configured to creation / build of messagebus
	/// </summary>
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
