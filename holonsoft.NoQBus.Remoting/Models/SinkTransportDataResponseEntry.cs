namespace holonsoft.NoQBus.Remoting.Models;

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
