namespace holonsoft.NoQBus.Tests.TestDtoClasses
{
	public record TestRequest : RequestBase<TestResponse> { }
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
}
