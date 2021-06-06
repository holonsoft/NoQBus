using holonsoft.Utils;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace holonsoft.NoQBus
{
	public abstract class MessageBusSinkBase : IMessageBusSink
	{
		private IRemoteMessageBus _messageBus;
		public void SetMessageBus(IRemoteMessageBus messageBus)
		{
			_messageBus = _messageBus == null ? messageBus : throw new NotSupportedException($"MessageBus for the {nameof(MessageBusSinkBase)} is already set!");
		}
		protected IRemoteMessageBus EnsureMessageBus()
			 => _messageBus ?? throw new NotSupportedException($"MessageBus for the {nameof(MessageBusSinkBase)} is not set!");

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

		public abstract Task StartAsync(CancellationToken cancellationToken = default);

		public abstract Task<SinkTransportDataResponse> TransportToEndpoint(SinkTransportDataRequest request);

		public async Task<IResponse[]> GetResponses(IRequest request)
		{
			byte[] serializedRequest = _encoding.GetBytes(JsonSerializer.Serialize(request, request.GetType(), _serializerOptions));
			var response = await TransportToEndpoint(new SinkTransportDataRequest(request.GetType().FullName, serializedRequest));
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
				var responses = await EnsureMessageBus().GetResponsesForRemotedRequest(deserializedRequest);
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
