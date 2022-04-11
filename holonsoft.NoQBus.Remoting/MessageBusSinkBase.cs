using holonsoft.FluentConditions;
using holonsoft.NoQBus.Abstractions.Contracts;
using holonsoft.NoQBus.Remoting.Models;
using holonsoft.Utils;

namespace holonsoft.NoQBus.Remoting;
public abstract class MessageBusSinkBase : IMessageBusSink
{
  protected MessageBusSinkBase(IMessageSerializer messageSerializer)
    => _messageSerializer = messageSerializer;

  private IRemoteMessageBus _messageBus;
  private readonly IMessageSerializer _messageSerializer;

  public void SetMessageBus(IRemoteMessageBus messageBus)
    => _messageBus = _messageBus == null ? messageBus : throw new NotSupportedException($"MessageBus for the {nameof(MessageBusSinkBase)} is already set!");
  protected IRemoteMessageBus EnsureMessageBus()
     => _messageBus ?? throw new NotSupportedException($"MessageBus for the {nameof(MessageBusSinkBase)} is not set!");

  public abstract Task StartAsync(CancellationToken cancellationToken = default);

  public abstract Task<SinkTransportDataResponse> TransportToEndpoint(SinkTransportDataRequest request);

  public async Task<IResponse[]> GetResponses(IRequest request)
  {
    var serializedRequest = _messageSerializer.Serialize(request);
    var response = await TransportToEndpoint(new SinkTransportDataRequest(request.GetType().FullName, serializedRequest));
    return
       response.ResponseEntries
               .AsParallel()
               .Select(DeserializeEntry)
               .ToArray();

    IResponse DeserializeEntry(SinkTransportDataResponseEntry entry)
    {
      if (ReflectionUtils.AllNonAbstractTypes.TryGetValue(entry.TypeName, out var responseType))
      {
        responseType.Requires(nameof(responseType)).IsOfType<IResponse>();

        return (IResponse) _messageSerializer.Deserialize(responseType, entry.SerializedRequestMessage);
      }
      throw new InvalidOperationException($"Could not deserialize type {entry.TypeName} - type not found!");
    }
  }

  public async Task<SinkTransportDataResponse> GetResponsesForRemoteRequest(SinkTransportDataRequest request)
  {
    if (ReflectionUtils.AllNonAbstractTypes.TryGetValue(request.TypeName, out var requestType))
    {
      requestType.Requires(nameof(requestType)).IsOfType<IRequest>();

      var deserializedRequest = (IRequest) _messageSerializer.Deserialize(requestType, request.SerializedRequestMessage);
      var responses = await EnsureMessageBus().GetResponsesForRemoteRequest(deserializedRequest);
      return new SinkTransportDataResponse(request, responses.Select(SerializeEntry).ToArray());

    }
    throw new InvalidOperationException($"Could not deserialize type {request.TypeName} - type not found!");

    SinkTransportDataResponseEntry SerializeEntry(IResponse entry)
      => new SinkTransportDataResponseEntry(entry.GetType().FullName, _messageSerializer.Serialize(entry));
  }
}
