using holonsoft.NoQBus.Abstractions.Models;
using System.Threading;
using System.Threading.Tasks;

namespace holonsoft.NoQBus.Abstractions.Contracts
{
	/// <summary>
	/// Interface to configure and handle remoting of messagebus
	/// </summary>
	public interface IMessageBusSink
	{
		/// <summary>
		/// Inject remoting part into standard message bus
		/// </summary>
		/// <param name="messageBus"><see cref="IRemoteMessageBus"/></param>
		public void SetMessageBus(IRemoteMessageBus messageBus);

		/// <summary>
		/// Start remoting
		/// </summary>
		/// <param name="cancellationToken">a cancellationtoken to interrupt processing</param>
		/// <returns>A simple task</returns>
		public Task StartAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// Get an array of responses after sending a request to an endpoint
		/// </summary>
		/// <param name="request">Request message</param>
		/// <returns>A task with an array of responses, <see cref="IResponse"/></returns>
		public Task<IResponse[]> GetResponses(IRequest request);

		/// <summary>
		/// Get an array of responses after sending a request to an endpoint in a single wrapper message
		/// </summary>
		/// <param name="request">Wrapped (for transport) request message</param>
		/// <returns><see cref="SinkTransportDataResponse"/></returns>
		public Task<SinkTransportDataResponse> GetResponsesForRemoteRequest(SinkTransportDataRequest request);
	}
}
