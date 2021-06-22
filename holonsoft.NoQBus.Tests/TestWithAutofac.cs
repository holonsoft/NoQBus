using Autofac;
using FluentAssertions;
using holonsoft.NoQBus.SignalR.Client;
using holonsoft.NoQBus.SignalR.Host;
using holonsoft.NoQBus.Tests.TestDtoClasses;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace holonsoft.NoQBus.Tests
{
	public class TestWithAutofac
	{
		[Fact]
		public async void TestSimpleLocalMessage()
		{
			ContainerBuilder containerBuilder = new();
			containerBuilder.AddNoQMessageBus();
			await using var lifetimeScope = containerBuilder.Build();
			await lifetimeScope.StartLocalNoQMessageBus();

			IMessageBus messageBus = lifetimeScope.Resolve<IMessageBus>();

			const string testString = "Test4711";
			static Task<TestResponse> ReceiveTestRequest(TestRequest request)
			{
				return Task.FromResult(new TestResponse(request, testString));
			}

			await messageBus.Subscribe<TestRequest, TestResponse>(ReceiveTestRequest);

			TestRequest sendRequest = new();

			TestResponse[] receivedResponse = await messageBus.GetResponses<TestResponse>(sendRequest);

			receivedResponse.Should().HaveCount(1);

			receivedResponse[0].CorrospondingRequestMessageId.Should().Be(sendRequest.MessageId);
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

				IMessageBus messageBusServer = lifetimeScopeServer.Resolve<IMessageBus>();

				ContainerBuilder containerBuilderClient = new();
				containerBuilderClient.AddNoQMessageBus();
				containerBuilderClient.AddNoQSignalRClient();
				await using var lifetimeScopeClient = containerBuilderClient.Build();

				await lifetimeScopeClient.StartNoQSignalRClient(cancellationToken: cts.Token);

				IMessageBus messageBusClient = lifetimeScopeClient.Resolve<IMessageBus>();

				const string testString = "Test4711";
				static Task<TestResponse> ReceiveTestRequest(TestRequest request)
				{
					return Task.FromResult(new TestResponse(request, testString));
				}

				await messageBusClient.Subscribe<TestRequest, TestResponse>(ReceiveTestRequest);

				TestRequest sendRequest = new();

				TestResponse[] receivedResponse = await messageBusServer.GetResponses<TestResponse>(sendRequest);

				receivedResponse.Should().HaveCount(1);

				receivedResponse[0].CorrospondingRequestMessageId.Should().Be(sendRequest.MessageId);
				receivedResponse[0].TestString.Should().Be(testString);
			}
			finally
			{
				cts.Cancel();
			}
		}
	}
}
