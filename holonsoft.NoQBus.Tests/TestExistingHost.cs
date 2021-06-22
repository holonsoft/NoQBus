using FluentAssertions;
using holonsoft.NoQBus.SignalR.Client;
using holonsoft.NoQBus.SignalR.Host;
using holonsoft.NoQBus.Tests.TestDtoClasses;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Xunit;


namespace holonsoft.NoQBus.Tests
{
	public class TestExistingHost
	{
		[Fact]
		public async void TestMessageSendFromClientToServerWithExistingHost()
		{
			CancellationTokenSource cts = new();
			var host = new HostBuilder()
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseUrls("http://localhost:5001");

					webBuilder.ConfigureServices(services =>
					{
						services.AddSignalR(o => o.EnableDetailedErrors = true);
						services.AddNoQMessageBus();
						services.AddNoQSignalRHost();
					});

					webBuilder.Configure(app =>
					{
						app.UseRouting();

						app.UseEndpoints(endpoints =>
						{
							endpoints.MapNoQSignalRHost("/NoQ/SignalR");
						});
					});
				})
				.Build();
			try
			{
				await host.StartAsync(cts.Token);

				await host.StartNoQSignalRHostOnExistingAspNetCoreHost(cancellationToken: cts.Token);

				MessageBus messageBusImplServer = host.Services.GetRequiredService<MessageBus>();
				MessageBus messageBusImplClient = new(new MessageBusSignalRClient());

				IMessageBusConfig messageBusConfig = messageBusImplClient;
				await messageBusConfig.StartNoQSignalRClient(cancellationToken: cts.Token);

				IMessageBus messageBusServer = messageBusImplServer;
				IMessageBus messageBusClient = messageBusImplClient;

				const string testString = "Test4711";
				static Task<TestResponse> ReceiveTestRequest(TestRequest request)
				{
					return Task.FromResult(new TestResponse(request, testString));
				}

				await messageBusServer.Subscribe<TestRequest, TestResponse>(ReceiveTestRequest);

				TestRequest sendRequest = new();

				TestResponse[] receivedResponse = await messageBusClient.GetResponses<TestResponse>(sendRequest);

				receivedResponse.Should().HaveCount(1);

				receivedResponse[0].CorrospondingRequestMessageId.Should().Be(sendRequest.MessageId);
				receivedResponse[0].TestString.Should().Be(testString);
			}
			finally
			{
				cts.Cancel();
				await host.StopAsync();
			}
		}
	}
}
