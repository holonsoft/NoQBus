using System;

namespace holonsoft.NoQBus.SignalR
{
	public class MessageBusSignalRConstants
	{
		public const string DefaultUrl = "http://localhost:5001/NoQ/SignalR";
		public static readonly TimeSpan DefaultRetryDelay = TimeSpan.FromSeconds(1);
		public const int DefaultInitialRetryCount = 10;
	}
}
