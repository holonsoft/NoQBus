using System;
using System.Threading;
using System.Threading.Tasks;

namespace holonsoft.NoQBus
{

	public interface IMessageBusConfigure
	{
		public IMessageBusConfigure SetTimeoutTimeSpan(TimeSpan timeOutTimeSpan);
		public IMessageBusConfigure AsClient();
		public IMessageBusConfigure AsServer();
		public IMessageBusConfigure ConfigureSink(Action<IMessageBusSink> sinkConfig);
		public Task StartAsync(CancellationToken cancellationToken = default);
	}
}