namespace holonsoft.NoQBus.Abstractions.Models;

public record SinkTransportDataRequest
{
  public Guid RequestIdentifier { get; init; }
  public string TypeName { get; init; }
  public byte[] SerializedRequestMessage { get; init; }

  public SinkTransportDataRequest() { } //for serializer

  public SinkTransportDataRequest(string typeName, byte[] serializedRequestMessage) : this()
  {
    RequestIdentifier = Guid.NewGuid();
    TypeName = typeName;
    SerializedRequestMessage = serializedRequestMessage;
  }
}
