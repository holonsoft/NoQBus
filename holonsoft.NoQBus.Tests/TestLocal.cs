using FluentAssertions;
using holonsoft.NoQBus.Abstractions.Contracts;
using holonsoft.NoQBus.Tests.TestDtoClasses;
using Xunit;

namespace holonsoft.NoQBus.Tests;

public partial class TestLocal
{
  [Fact]
  public async void TestSimpleLocalMessage()
  {
    MessageBus messageBusImpl = new();

    IMessageBusConfig messageBusConfig = messageBusImpl;
    await messageBusConfig
            .Configure()
            .StartAsync();

    IMessageBus messageBus = messageBusImpl;

    const string testString = "Test4711";
    static Task<TestResponse> ReceiveTestRequest(TestRequest request) => Task.FromResult(new TestResponse(request, testString));

    await messageBus.Subscribe<TestRequest, TestResponse>(ReceiveTestRequest);

    TestRequest sendRequest = new();

    var receivedResponse = await messageBus.GetResponses<TestResponse>(sendRequest);

    receivedResponse.Should().HaveCount(1);

    receivedResponse[0].CorrespondingRequestMessageId.Should().Be(sendRequest.MessageId);
    receivedResponse[0].TestString.Should().Be(testString);
  }
}
