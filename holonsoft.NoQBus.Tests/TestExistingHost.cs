using FluentAssertions;
using holonsoft.NoQBus.Abstractions.Contracts;
using holonsoft.NoQBus.Extensions.Microsoft;
using holonsoft.NoQBus.Serialization;
using holonsoft.NoQBus.SignalR.Client;
using holonsoft.NoQBus.SignalR.Host;
using holonsoft.NoQBus.SignalR.Host.Extensions.Microsoft;
using holonsoft.NoQBus.Tests.TestDtoClasses;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;


namespace holonsoft.NoQBus.Tests;

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

      var messageBusImplServer = host.Services.GetRequiredService<MessageBus>();
      MessageBus messageBusImplClient = new(new MessageBusSignalRClient(new MessageSerializer()));

      IMessageBusConfig messageBusConfig = messageBusImplClient;
      await messageBusConfig.StartNoQSignalRClient(x => x.UseUrl("http://localhost:5001/NoQ/SignalR"), cancellationToken: cts.Token);

      IMessageBus messageBusServer = messageBusImplServer;
      IMessageBus messageBusClient = messageBusImplClient;

      const string testString = "Test4711";
      static Task<TestResponse> ReceiveTestRequest(TestRequest request) => Task.FromResult(new TestResponse(request, testString));

      await messageBusServer.Subscribe<TestRequest, TestResponse>(ReceiveTestRequest);

      TestRequest sendRequest = new();

      var receivedResponse = await messageBusClient.GetResponses<TestResponse>(sendRequest);

      receivedResponse.Should().HaveCount(1);

      receivedResponse[0].CorrespondingRequestMessageId.Should().Be(sendRequest.MessageId);
      receivedResponse[0].TestString.Should().Be(testString);
    }
    finally
    {
      cts.Cancel();
      await host.StopAsync();
    }
  }
}
