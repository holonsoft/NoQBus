﻿using FluentAssertions;
using holonsoft.NoQBus.SignalR.Client;
using holonsoft.NoQBus.SignalR.Host;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace holonsoft.NoQBus.Tests
{
	public class TestSignalRClientServer
	{
		[Fact]
		public async void TestMessageSendFromClientToServer()
		{
			CancellationTokenSource cts = new();
			try
			{
				MessageBus messageBusImplServer = new(new MessageBusSignalRHost());
				MessageBus messageBusImplClient = new(new MessageBusSignalRClient());

				IMessageBusConfig messageBusConfig = messageBusImplServer;
				await messageBusConfig.StartSignalRHost(x => { }, cts.Token);

				messageBusConfig = messageBusImplClient;
				await messageBusConfig.StartSignalRClient(x => { }, cts.Token);

				IMessageBus messageBusServer = messageBusImplServer;
				IMessageBus messageBusClient = messageBusImplClient;

				const string testString = "Test4711";
				static Task<TestResponse> ReceiveTestRequest(TestRequest request)
				{
					return Task.FromResult(new TestResponse(request, testString));
				}

				await messageBusServer.Subscribe<TestRequest, TestResponse>(ReceiveTestRequest);

				TestRequest sendRequest = new();

				TestResponse[] receivedResponse = await messageBusClient.GetResponses<TestRequest, TestResponse>(sendRequest);

				receivedResponse.Should().HaveCount(1);

				receivedResponse[0].CorrospondingRequestMessageId.Should().Be(sendRequest.MessageId);
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
				MessageBus messageBusImplServer = new(new MessageBusSignalRHost());
				MessageBus messageBusImplClient = new(new MessageBusSignalRClient());

				IMessageBusConfig messageBusConfig = messageBusImplServer;
				await messageBusConfig.StartSignalRHost(x => { }, cts.Token);

				messageBusConfig = messageBusImplClient;
				await messageBusConfig.StartSignalRClient(x => { }, cts.Token);

				IMessageBus messageBusServer = messageBusImplServer;
				IMessageBus messageBusClient = messageBusImplClient;

				const string testString = "Test4711";
				static Task<TestResponse> ReceiveTestRequest(TestRequest request)
				{
					return Task.FromResult(new TestResponse(request, testString));
				}

				await messageBusClient.Subscribe<TestRequest, TestResponse>(ReceiveTestRequest);

				TestRequest sendRequest = new();

				TestResponse[] receivedResponse = await messageBusServer.GetResponses<TestRequest, TestResponse>(sendRequest);

				receivedResponse.Should().HaveCount(1);

				receivedResponse[0].CorrospondingRequestMessageId.Should().Be(sendRequest.MessageId);
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
				MessageBus messageBusImplServer = new(new MessageBusSignalRHost());

				IMessageBusConfig messageBusConfig = messageBusImplServer;
				await messageBusConfig.StartSignalRHost(x => { }, cts.Token);

				IMessageBus messageBusServer = messageBusImplServer;

				TestRequest sendRequest = new();

				TestResponse[] receivedResponse = await messageBusServer.GetResponses<TestRequest, TestResponse>(sendRequest);

				receivedResponse.Should().HaveCount(0);
			}
			finally
			{
				cts.Cancel();
			}
		}
	}
}