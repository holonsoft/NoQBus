using holonsoft.FluentConditions;
using holonsoft.NoQBus.PolymorphyHelper;
using holonsoft.Utils;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;
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

		public static JsonSerializerOptions CreateSerializerOptions()
		{
			JsonSerializerOptions options = new()
			{
				ReferenceHandler = new RootedPreserveReferenceHandler(), //for the converter - thats why have all the time to create this JsonSerializerOptions new!
				WriteIndented = true
			};

			options.Converters.Add(new JsonSerializerPolymorphyConverter());

			return options;
		}

		private readonly Encoding _encoding = Encoding.UTF8;

		public abstract Task StartAsync(CancellationToken cancellationToken = default);

		public abstract Task<SinkTransportDataResponse> TransportToEndpoint(SinkTransportDataRequest request);

		public async Task<IResponse[]> GetResponses(IRequest request)
		{
			byte[] serializedRequest = _encoding.GetBytes(JsonSerializer.Serialize(request, request.GetType(), CreateSerializerOptions()));
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
					responseType.Requires(nameof(responseType)).IsOfType<IResponse>();

					return (IResponse) JsonSerializer.Deserialize(_encoding.GetString(entry.SerializedRequestMessage), responseType, CreateSerializerOptions());
				}
				throw new InvalidOperationException($"Could not deserialize type {entry.TypeName} - type not found!");
			}
		}

		public async Task<SinkTransportDataResponse> GetResponsesForRemotedRequest(SinkTransportDataRequest request)
		{
			if (ReflectionUtils.AllNonAbstractTypes.TryGetValue(request.TypeName, out Type requestType))
			{
				requestType.Requires(nameof(requestType)).IsOfType<IRequest>();

				var deserializedRequest = (IRequest) JsonSerializer.Deserialize(_encoding.GetString(request.SerializedRequestMessage), requestType, CreateSerializerOptions());
				var responses = await EnsureMessageBus().GetResponsesForRemotedRequest(deserializedRequest);
				return new SinkTransportDataResponse(request, responses.Select(SerializeEntry).ToArray());

			}
			throw new InvalidOperationException($"Could not deserialize type {request.TypeName} - type not found!");

			SinkTransportDataResponseEntry SerializeEntry(IResponse entry)
			{
				return new SinkTransportDataResponseEntry(entry.GetType().FullName, _encoding.GetBytes(JsonSerializer.Serialize(entry, entry.GetType(), CreateSerializerOptions())));
			}
		}
	}
}
