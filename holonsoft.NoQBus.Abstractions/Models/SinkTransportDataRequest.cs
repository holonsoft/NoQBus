using System;

namespace holonsoft.NoQBus.Abstractions.Models
{
	public record SinkTransportDataRequest
	{
#if NET5_0_OR_GREATER
		public Guid RequestIdentifier { get; init; }
		public string TypeName { get; init; }
		public byte[] SerializedRequestMessage { get; init; }
#else
		public Guid RequestIdentifier { get; set; }
		public string TypeName { get; set; }
		public byte[] SerializedRequestMessage { get; set; }
#endif


		public SinkTransportDataRequest() { } //for serializer

		public SinkTransportDataRequest(string typeName, byte[] serializedRequestMessage) : this()
		{
			RequestIdentifier = Guid.NewGuid();
			TypeName = typeName;
			SerializedRequestMessage = serializedRequestMessage;
		}
	}
}
