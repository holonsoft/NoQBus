using FluentAssertions;
using holonsoft.NoQBus.Abstractions.Contracts;
using holonsoft.NoQBus.Tests.TestDtoClasses;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace holonsoft.NoQBus.Tests
{
	public class TestFiltering
	{
		[Fact]
		public async void TestOneRequestFilter()
		{
			MessageBus messageBusImpl = new();

			IMessageBusConfig messageBusConfig = messageBusImpl;
			await messageBusConfig
							.Configure()
							.StartAsync();

			IMessageBusFiltering messageBusFiltering = messageBusImpl;

			IMessageBus messageBus = messageBusImpl;

			const string testString = "Test4711";
			static Task<TestResponse> ReceiveTestRequest(TestRequest request)
			{
				return Task.FromResult(new TestResponse(request, testString));
			}

			await messageBus.Subscribe<TestRequest, TestResponse>(ReceiveTestRequest);

			var requestFilterId = await messageBusFiltering.AddRequestFilter<TestRequest>(x => Task.FromResult(false));

			TestRequest sendRequest = new();

			TestResponse[] receivedResponse = await messageBus.GetResponses<TestResponse>(sendRequest);

			receivedResponse.Should().HaveCount(0);

			await messageBusFiltering.RemoveRequestFilter(requestFilterId);

			sendRequest = new();

			receivedResponse = await messageBus.GetResponses<TestResponse>(sendRequest);

			receivedResponse.Should().HaveCount(1);

			receivedResponse[0].CorrospondingRequestMessageId.Should().Be(sendRequest.MessageId);
			receivedResponse[0].TestString.Should().Be(testString);
		}

		[Fact]
		public async void TestTwoRequestFilter()
		{
			MessageBus messageBusImpl = new();

			IMessageBusConfig messageBusConfig = messageBusImpl;
			await messageBusConfig
							.Configure()
							.StartAsync();

			IMessageBusFiltering messageBusFiltering = messageBusImpl;

			IMessageBus messageBus = messageBusImpl;

			const string testString = "Test4711";
			static Task<TestResponse> ReceiveTestRequest(TestRequest request)
			{
				return Task.FromResult(new TestResponse(request, testString));
			}

			await messageBus.Subscribe<TestRequest, TestResponse>(ReceiveTestRequest);

			var requestFilterId = await messageBusFiltering.AddRequestFilter<TestRequest>(x => Task.FromResult(true));

			TestRequest sendRequest = new();

			TestResponse[] receivedResponse = await messageBus.GetResponses<TestResponse>(sendRequest);

			receivedResponse.Should().HaveCount(1);

			receivedResponse[0].CorrospondingRequestMessageId.Should().Be(sendRequest.MessageId);
			receivedResponse[0].TestString.Should().Be(testString);

			var requestFilterId2 = await messageBusFiltering.AddRequestFilter<TestRequest>(x => Task.FromResult(false));

			sendRequest = new();

			receivedResponse = await messageBus.GetResponses<TestResponse>(sendRequest);

			receivedResponse.Should().HaveCount(0);

			await messageBusFiltering.RemoveRequestFilter(requestFilterId2);

			sendRequest = new();

			receivedResponse = await messageBus.GetResponses<TestResponse>(sendRequest);

			receivedResponse.Should().HaveCount(1);

			receivedResponse[0].CorrospondingRequestMessageId.Should().Be(sendRequest.MessageId);
			receivedResponse[0].TestString.Should().Be(testString);
		}

		[Fact]
		public async void TestOneResponseFilter()
		{
			MessageBus messageBusImpl = new();

			IMessageBusConfig messageBusConfig = messageBusImpl;
			await messageBusConfig
							.Configure()
							.StartAsync();

			IMessageBusFiltering messageBusFiltering = messageBusImpl;

			IMessageBus messageBus = messageBusImpl;

			const string testString = "Test4711";
			static Task<TestResponse> ReceiveTestRequest(TestRequest request)
			{
				return Task.FromResult(new TestResponse(request, testString));
			}

			await messageBus.Subscribe<TestRequest, TestResponse>(ReceiveTestRequest);

			var responseFilterId = await messageBusFiltering.AddResponseFilter<TestResponse>(x => Task.FromResult(x.Where(z => false)));

			TestRequest sendRequest = new();

			TestResponse[] receivedResponse = await messageBus.GetResponses<TestResponse>(sendRequest);

			receivedResponse.Should().HaveCount(0);

			await messageBusFiltering.RemoveResponseFilter(responseFilterId);

			sendRequest = new();

			receivedResponse = await messageBus.GetResponses<TestResponse>(sendRequest);

			receivedResponse.Should().HaveCount(1);

			receivedResponse[0].CorrospondingRequestMessageId.Should().Be(sendRequest.MessageId);
			receivedResponse[0].TestString.Should().Be(testString);
		}

		[Fact]
		public async void TestTwoResponseFilter()
		{
			MessageBus messageBusImpl = new();

			IMessageBusConfig messageBusConfig = messageBusImpl;
			await messageBusConfig
							.Configure()
							.StartAsync();

			IMessageBusFiltering messageBusFiltering = messageBusImpl;

			IMessageBus messageBus = messageBusImpl;

			const string testString = "Test4711";
			static Task<TestResponse> ReceiveTestRequest(TestRequest request)
			{
				return Task.FromResult(new TestResponse(request, testString));
			}

			await messageBus.Subscribe<TestRequest, TestResponse>(ReceiveTestRequest);

			var responseFilterId = await messageBusFiltering.AddResponseFilter<TestResponse>(x => Task.FromResult(x));

			TestRequest sendRequest = new();

			TestResponse[] receivedResponse = await messageBus.GetResponses<TestResponse>(sendRequest);

			receivedResponse.Should().HaveCount(1);

			receivedResponse[0].CorrospondingRequestMessageId.Should().Be(sendRequest.MessageId);
			receivedResponse[0].TestString.Should().Be(testString);

			var responseFilterId2 = await messageBusFiltering.AddResponseFilter<TestResponse>(x => Task.FromResult(x.Where(z => false)));

			sendRequest = new();

			receivedResponse = await messageBus.GetResponses<TestResponse>(sendRequest);

			receivedResponse.Should().HaveCount(0);

			await messageBusFiltering.RemoveResponseFilter(responseFilterId2);

			sendRequest = new();

			receivedResponse = await messageBus.GetResponses<TestResponse>(sendRequest);

			receivedResponse.Should().HaveCount(1);

			receivedResponse[0].CorrospondingRequestMessageId.Should().Be(sendRequest.MessageId);
			receivedResponse[0].TestString.Should().Be(testString);
		}
	}
}
