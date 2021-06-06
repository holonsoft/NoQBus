using System;

namespace holonsoft.NoQBus.SignalR.Client
{
	public interface IMessageBusSignalRClientConfig
	{
		public IMessageBusSignalRClientConfig UseUrl(string url);

		public IMessageBusSignalRClientConfig SetRetryDelay(TimeSpan retryDelay);

		public IMessageBusSignalRClientConfig SetInitialRetryCount(int initialRetryCount);
	}
}