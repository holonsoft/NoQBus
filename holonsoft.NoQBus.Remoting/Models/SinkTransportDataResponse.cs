namespace holonsoft.NoQBus.Remoting.Models;

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
