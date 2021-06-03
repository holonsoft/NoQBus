using holonsoft.Utils;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace holonsoft.NoQBus
{
	public class MessageBusSink : IMessageBusSink
	{
		protected IMessageBusSinkTransport _transport;
		private readonly IRemoteMessageBus _messageBus;

		public MessageBusSink(IMessageBusSinkTransport transport, IRemoteMessageBus messageBus)
		{
			_transport = transport;
			_messageBus = messageBus;
		}

		private readonly JsonSerializerOptions _serializerOptions
			 = new()
			 {
				 ReferenceHandler = ReferenceHandler.Preserve,
#if DEBUG
				 WriteIndented = true
#else
				 WriteIndented = false
#endif
			 };

		private readonly Encoding _encoding = Encoding.UTF8;

		public async Task<IResponse[]> GetResponses(IRequest request)
		{
			byte[] serializedRequest = _encoding.GetBytes(JsonSerializer.Serialize(request, request.GetType(), _serializerOptions));
			var response = await _transport.TransportToEndpoint(new SinkTransportDataRequest(request.GetType().FullName, serializedRequest));
			return
				 response.ResponseEntries
								 .AsParallel()
								 .Select(DeserializeEntry)
								 .ToArray();

			IResponse DeserializeEntry(SinkTransportDataResponseEntry entry)
			{
				if (ReflectionUtils.AllNonAbstractTypes.TryGetValue(entry.TypeName, out Type responseType))
				{
					return (IResponse) JsonSerializer.Deserialize(_encoding.GetString(entry.SerializedRequestMessage), responseType, _serializerOptions);
				}
				throw new InvalidOperationException($"Could not deserialize type {entry.TypeName} - type not found!");
			}
		}

		public async Task<SinkTransportDataResponse> GetResponsesForRemotedRequest(SinkTransportDataRequest request)
		{
			if (ReflectionUtils.AllNonAbstractTypes.TryGetValue(request.TypeName, out Type requestType))
			{
				var deserializedRequest = (IRequest) JsonSerializer.Deserialize(_encoding.GetString(request.SerializedRequestMessage), requestType, _serializerOptions);
				var responses = await _messageBus.GetResponsesForRemotedRequest(deserializedRequest);
				return new SinkTransportDataResponse(request, responses.Select(SerializeEntry).ToArray());

			}
			throw new InvalidOperationException($"Could not deserialize type {request.TypeName} - type not found!");

			SinkTransportDataResponseEntry SerializeEntry(IResponse entry)
			{
				return new SinkTransportDataResponseEntry(entry.GetType().FullName, _encoding.GetBytes(JsonSerializer.Serialize(entry, entry.GetType(), _serializerOptions)));
			}
		}
	}
}
