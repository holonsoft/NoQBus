using Autofac;
using FluentAssertions;
using holonsoft.NoQBus.Abstractions.Contracts;
using holonsoft.NoQBus.Extensions.Autofac;
using holonsoft.NoQBus.SignalR.Client;
using holonsoft.NoQBus.SignalR.Client.Extensions.Autofac;
using holonsoft.NoQBus.SignalR.Host;
using holonsoft.NoQBus.SignalR.Host.Extensions.Autofac;
using holonsoft.NoQBus.Tests.TestDtoClasses;
using Xunit;

namespace holonsoft.NoQBus.Tests;

public class TestWithAutofac
{
  [Fact]
  public async void TestSimpleLocalMessage()
  {
    ContainerBuilder containerBuilder = new();
    containerBuilder.AddNoQMessageBus();
    await using var lifetimeScope = containerBuilder.Build();
    await lifetimeScope.StartLocalNoQMessageBus();

    var messageBus = lifetimeScope.Resolve<IMessageBus>();

    const string testString = "Test4711";
    static Task<TestResponse> ReceiveTestRequest(TestRequest request) => Task.FromResult(new TestResponse(request, testString));

    await messageBus.Subscribe<TestRequest, TestResponse>(ReceiveTestRequest);

    TestRequest sendRequest = new();

    var receivedResponse = await messageBus.GetResponses<TestResponse>(sendRequest);

    receivedResponse.Should().HaveCount(1);

    receivedResponse[0].CorrespondingRequestMessageId.Should().Be(sendRequest.MessageId);
    receivedResponse[0].TestString.Should().Be(testString);
  }

  [Fact]
  public async void TestMessageSendFromServerToClient()
  {
    CancellationTokenSource cts = new();
    try
    {
      ContainerBuilder containerBuilderServer = new();
      containerBuilderServer.AddNoQMessageBus();
      containerBuilderServer.AddNoQSignalRHost();
      await using var lifetimeScopeServer = containerBuilderServer.Build();

      await lifetimeScopeServer.StartNoQSignalRHost(cancellationToken: cts.Token);

      var messageBusServer = lifetimeScopeServer.Resolve<IMessageBus>();

      ContainerBuilder containerBuilderClient = new();
      containerBuilderClient.AddNoQMessageBus();
      containerBuilderClient.AddNoQSignalRClient();
      await using var lifetimeScopeClient = containerBuilderClient.Build();

      await lifetimeScopeClient.StartNoQSignalRClient(cancellationToken: cts.Token);

      var messageBusClient = lifetimeScopeClient.Resolve<IMessageBus>();

      const string testString = "Test4711";
      static Task<TestResponse> ReceiveTestRequest(TestRequest request) => Task.FromResult(new TestResponse(request, testString));

      await messageBusClient.Subscribe<TestRequest, TestResponse>(ReceiveTestRequest);

      TestRequest sendRequest = new();

      var receivedResponse = await messageBusServer.GetResponses<TestResponse>(sendRequest);

      receivedResponse.Should().HaveCount(1);

      receivedResponse[0].CorrespondingRequestMessageId.Should().Be(sendRequest.MessageId);
      receivedResponse[0].TestString.Should().Be(testString);
    }
    finally
    {
      cts.Cancel();
    }
  }
}
