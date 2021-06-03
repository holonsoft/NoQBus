using FluentAssertions;
using holonsoft.NoQBus;
using System.Threading.Tasks;
using Xunit;

namespace NoQBus.Tests
{
	public class UnitTest1
	{
		public record TestResponse : ResponseBase<TestRequest>
		{
			public string TestString { get; init; }

			public TestResponse()
			{
			}

			public TestResponse(TestRequest cloneFromRequest, string testString) : base(cloneFromRequest)
			{
				TestString = testString;
			}
		}
		public record TestRequest : RequestBase<TestResponse> { }

		[Fact]
		public async void TestSimpleLocalMessage()
		{
			MessageBus messageBusImpl = new();

			IMessageBusConfig messageBusConfig = messageBusImpl;
			messageBusConfig
				.Configure()
				.ConfigureForLocalUse()
				.Build();

			IMessageBus messageBus = messageBusImpl;

			const string testString = "Test4711";
			static Task<TestResponse> ReceiveTestRequest(TestRequest request)
			{
				return Task.FromResult(new TestResponse(request, testString));
			}

			await messageBus.Subscribe<TestRequest, TestResponse>(ReceiveTestRequest);

			TestRequest sendRequest = new();

			TestResponse[] receivedResponse = await messageBus.GetResponses<TestRequest, TestResponse>(sendRequest);

			receivedResponse.Should().HaveCount(1);

			receivedResponse[0].CorrospondingRequestMessageId.Should().Be(sendRequest.MessageId);
			receivedResponse[0].TestString.Should().Be(testString);
		}
	}
}
