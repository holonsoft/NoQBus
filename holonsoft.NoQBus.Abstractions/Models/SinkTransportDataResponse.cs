using System;

namespace holonsoft.NoQBus
{
	public class SinkTransportDataResponse
	{
		public Guid RequestIdentifier { get; init; }

		public SinkTransportDataResponseEntry[] ResponseEntries { get; init; }

		public SinkTransportDataResponse() { } //for serializer

		public SinkTransportDataResponse(Guid requestIdentifier, params SinkTransportDataResponseEntry[] responseEntries) : this()
		{
			RequestIdentifier = requestIdentifier;
			ResponseEntries = responseEntries;
		}

		public SinkTransportDataResponse(SinkTransportDataRequest request, params SinkTransportDataResponseEntry[] responseEntries)
			 : this(request.RequestIdentifier, responseEntries) { }
	}

	public class SinkTransportDataResponseEntry
	{
		public string TypeName { get; init; }
		public byte[] SerializedRequestMessage { get; init; }

		public SinkTransportDataResponseEntry() { } //for serializer

		public SinkTransportDataResponseEntry(string typeName, byte[] serializedRequestMessage) : this()
		{
			TypeName = typeName;
			SerializedRequestMessage = serializedRequestMessage;
		}
	}
}
