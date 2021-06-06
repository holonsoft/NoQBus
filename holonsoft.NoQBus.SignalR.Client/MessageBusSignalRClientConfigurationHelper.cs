using Autofac;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace holonsoft.NoQBus.SignalR.Client
{
	public static class MessageBusSignalRClientConfigurationHelper
	{
		public static void AddSignalRClient(this ContainerBuilder containerBuilder)
		{
			containerBuilder
				 .RegisterType<MessageBusSignalRClient>()
				 .As<MessageBusSignalRClient>()
				 .As<IMessageBusSignalRClientConfig>()
				 .As<IMessageBusSink>()
				 .SingleInstance();
		}

		public static Task StartSignalRClient(this IMessageBusConfig config,
																					Action<IMessageBusSignalRClientConfig> configure,
																					CancellationToken cancellationToken = default)
		{
			return
				config
					.Configure()
					.AsClient()
					.ConfigureSink(x => configure((IMessageBusSignalRClientConfig) x))
					.StartAsync(cancellationToken);
		}
	}
}
