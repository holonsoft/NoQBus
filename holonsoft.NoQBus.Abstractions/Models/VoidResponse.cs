namespace holonsoft.NoQBus.Abstractions.Models
{
	public record VoidResponse : ResponseBase
	{
		private record VoidMessage : MessageBase
		{
		}

		public static readonly VoidResponse Instance = new();
		public VoidResponse() : base(new VoidMessage()) { }
	}
}
