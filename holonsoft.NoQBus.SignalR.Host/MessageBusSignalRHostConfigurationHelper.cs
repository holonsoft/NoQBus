using Autofac;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace holonsoft.NoQBus.SignalR.Host
{
	public static class MessageBusSignalRHostConfigurationHelper
	{
		public static void AddSignalRHost(this ContainerBuilder containerBuilder)
		{
			containerBuilder
				 .RegisterType<MessageBusSignalRHost>()
				 .As<MessageBusSignalRHost>()
				 .As<IMessageBusSink>()
				 .As<IMessageBusSignalRHostConfig>()
				 .SingleInstance();
		}

		public static Task StartSignalRHost(this IMessageBusConfig config,
																				Action<IMessageBusSignalRHostConfig> configure,
																				CancellationToken cancellationToken = default)
		{
			return
				config
					.Configure()
					.AsServer()
					.ConfigureSink(x => configure((IMessageBusSignalRHostConfig) x))
					.StartAsync(cancellationToken);
		}
	}
}
