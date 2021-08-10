using System;
using System.Runtime.Serialization;
using holonsoft.NoQBus.Abstractions.Attributes;

namespace holonsoft.NoQBus.Abstractions.Exceptions
{
	/// <summary>
	/// Thrown if a message is decorated with<see cref="DenyRemotingAttribute">DenyRemotingAttribute</see>/>
	/// </summary>
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
