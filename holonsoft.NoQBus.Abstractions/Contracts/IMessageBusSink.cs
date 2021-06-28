using holonsoft.NoQBus.Abstractions.Models;
using System.Threading;
using System.Threading.Tasks;

namespace holonsoft.NoQBus.Abstractions.Contracts
{
	public interface IMessageBusSink
	{
		public void SetMessageBus(IRemoteMessageBus messageBus);

		public Task StartAsync(CancellationToken cancellationToken = default);

		public Task<IResponse[]> GetResponses(IRequest request);

		public Task<SinkTransportDataResponse> GetResponsesForRemotedRequest(SinkTransportDataRequest request);
	}
}
