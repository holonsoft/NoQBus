using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace holonsoft.NoQBus.SignalR.Host
{
	public partial class MessageBusSignalRHost : MessageBusSinkBase
	{
		private readonly MessageBusSignalRHubStateStore _stateStore = new();

		private IHubContext<MessageBusSignalRHub, IMessageBusSignalRClient> _hubContext;
		public void SetHubContext(IHubContext<MessageBusSignalRHub, IMessageBusSignalRClient> hubContext)
		{
			_hubContext = _hubContext == null ? hubContext : throw new NotSupportedException($"HubContext for the {nameof(MessageBusSignalRHost)} is already set!");
		}
		protected IHubContext<MessageBusSignalRHub, IMessageBusSignalRClient> EnsureHubContext()
			 => _hubContext ?? throw new NotSupportedException($"HubContext for the {nameof(MessageBusSignalRHost)} is not set!");

		public string Url { get; private set; } = MessageBusSignalRConstants.DefaultUrl;

		public async override Task<SinkTransportDataResponse> TransportToEndpoint(SinkTransportDataRequest request)
		{
			var hubContext = EnsureHubContext();

			string[] connections = _stateStore.GetConnections();

			if (connections.Length > 0)
			{
				Task<SinkTransportDataResponse> result = _stateStore.AwaitResultForRequest(request, connections);

				List<(string Connection, Task Task)> requestsToClient = new();
				foreach (string connection in connections)
				{
					try
					{
						var client = hubContext.Clients.Client(connection);

						requestsToClient.Add((connection, client.ProcessMessage(request)));
					}
					catch (Exception)
					{
						_stateStore.ClientDisconnected(connection);
					}
				}

				foreach (var requestToClient in requestsToClient)
				{
					try
					{
						await requestToClient.Task;
					}
					catch (Exception)
					{
						_stateStore.ClientDisconnected(requestToClient.Connection);
					}
				}

				return await result;
			}

			return new SinkTransportDataResponse(request);
		}

		public override async Task StartAsync(CancellationToken cancellationToken = default)
		{
			UriBuilder uriBuilder = new(Url);
			uriBuilder.Path = "";
			uriBuilder.Query = "";
			uriBuilder.Fragment = "";

			var host = new HostBuilder()
								 .ConfigureWebHostDefaults(webBuilder =>
								 {
									 webBuilder.UseUrls(uriBuilder.ToString());

									 webBuilder.ConfigureServices(services =>
									 {
										 services.AddSingleton(_stateStore);
										 services.AddSingleton(this);
										 services.AddSignalR(o => o.EnableDetailedErrors = true);
									 });

									 webBuilder.Configure(app =>
									 {
										 SetHubContext(app.ApplicationServices.GetRequiredService<IHubContext<MessageBusSignalRHub, IMessageBusSignalRClient>>());

										 app.UseRouting();

										 app.UseEndpoints(endpoints =>
										 {
											 endpoints.MapHub<MessageBusSignalRHub>(new UriBuilder(Url).Path);
										 });
									 });
								 })
								 .Build();

			await host.StartAsync(cancellationToken);

			async Task WaitForShutdownAndDispose()
			{
				await host.WaitForShutdownAsync(cancellationToken);
				host.Dispose();
			}

			_ = WaitForShutdownAndDispose();
		}
	}
}
