using holonsoft.NoQBus.Abstractions.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace holonsoft.NoQBus
{
	public partial class MessageBus : IMessageBusConfig, IMessageBusConfigure, IMessageBusConfigured
	{
		private bool _isConfigured;

		bool IMessageBusConfigured.IsConfigured => _isConfigured;


		IMessageBusConfigure IMessageBusConfig.Configure()
			=> _isConfigured
				? throw new NotSupportedException($"More than one configuration for the {nameof(MessageBus)} is not supported")
				: this;

		IMessageBusConfigure IMessageBusConfigure.SetTimeoutTimeSpan(TimeSpan timeOutTimeSpan)
		{
			_timeOutTimeSpan = timeOutTimeSpan;
			return this;
		}

		IMessageBusConfigure IMessageBusConfigure.ThrowIfNoReceiverSubscribed()
		{
			_throwIfNoReceiverSubscribed = true;
			return this;
		}

		IMessageBusConfigure IMessageBusConfigure.DoNotThrowIfNoReceiverSubscribed()
		{
			_throwIfNoReceiverSubscribed = false;
			return this;
		}

		IMessageBusConfigure IMessageBusConfigure.ConfigureSink(Action<IMessageBusSink> sinkConfig)
		{
			sinkConfig(_messageSink);
			return this;
		}

		async Task IMessageBusConfigure.StartAsync(CancellationToken cancellationToken)
		{
			_messageSink?.SetMessageBus(this);
			await (_messageSink?.StartAsync(cancellationToken) ?? Task.CompletedTask);
			_isConfigured = true;
		}


		private void EnsureConfigured()
		{
			if (!_isConfigured)
				throw new NotSupportedException($"The {nameof(MessageBus)} is not configured!");
		}
	}

}
