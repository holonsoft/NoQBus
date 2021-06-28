namespace holonsoft.NoQBus.Abstractions.Models
{
	public class SinkTransportDataResponseEntry
	{
#if NET5_0_OR_GREATER
		public string TypeName { get; init; }
		public byte[] SerializedRequestMessage { get; init; }
#else
		public string TypeName { get; set; }
		public byte[] SerializedRequestMessage { get; set; }
#endif

		public SinkTransportDataResponseEntry() { } //for serializer

		public SinkTransportDataResponseEntry(string typeName, byte[] serializedRequestMessage) : this()
		{
			TypeName = typeName;
			SerializedRequestMessage = serializedRequestMessage;
		}
	}
}