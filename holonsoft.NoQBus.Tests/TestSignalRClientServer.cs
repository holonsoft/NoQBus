using FluentAssertions;
using holonsoft.NoQBus.Abstractions.Contracts;
using holonsoft.NoQBus.SignalR.Client;
using holonsoft.NoQBus.SignalR.Host;
using holonsoft.NoQBus.Tests.TestDtoClasses;
using Xunit;

namespace holonsoft.NoQBus.Tests;

public class TestSignalRClientServer
{
  [Fact]
  public async void TestMessageSendFromClientToServer()
  {
    CancellationTokenSource cts = new();
    try
    {
      MessageBus messageBusImplServer = new(new MessageBusSignalRHost(new MessageBusSignalRHubStateStore()));
      MessageBus messageBusImplClient = new(new MessageBusSignalRClient());

      IMessageBusConfig messageBusConfig = messageBusImplServer;
      await messageBusConfig.StartNoQSignalRHost(cancellationToken: cts.Token);

      messageBusConfig = messageBusImplClient;
      await messageBusConfig.StartNoQSignalRClient(cancellationToken: cts.Token);

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
    }
  }

  [Fact]
  public async void TestMessageSendFromServerToClient()
  {
    CancellationTokenSource cts = new();
    try
    {
      MessageBus messageBusImplServer = new(new MessageBusSignalRHost(new MessageBusSignalRHubStateStore()));
      MessageBus messageBusImplClient = new(new MessageBusSignalRClient());

      IMessageBusConfig messageBusConfig = messageBusImplServer;
      await messageBusConfig.StartNoQSignalRHost(cancellationToken: cts.Token);

      messageBusConfig = messageBusImplClient;
      await messageBusConfig.StartNoQSignalRClient(cancellationToken: cts.Token);

      IMessageBus messageBusServer = messageBusImplServer;
      IMessageBus messageBusClient = messageBusImplClient;

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

  [Fact]
  public async void TestMessageSendFromServerButNoClientConnected()
  {
    CancellationTokenSource cts = new();
    try
    {
      MessageBus messageBusImplServer = new(new MessageBusSignalRHost(new MessageBusSignalRHubStateStore()));

      IMessageBusConfig messageBusConfig = messageBusImplServer;
      await messageBusConfig.StartNoQSignalRHost(cancellationToken: cts.Token);

      IMessageBus messageBusServer = messageBusImplServer;

      TestRequest sendRequest = new();

      var receivedResponse = await messageBusServer.GetResponses<TestResponse>(sendRequest);

      receivedResponse.Should().HaveCount(0);
    }
    finally
    {
      cts.Cancel();
    }
  }
}
