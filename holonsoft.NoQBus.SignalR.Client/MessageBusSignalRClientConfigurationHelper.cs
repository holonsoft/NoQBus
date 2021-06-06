using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace holonsoft.NoQBus.SignalR.Client
{
	public static class MessageBusSignalRClientConfigurationHelper
	{
		public static void AddNoQSignalRClient(this ContainerBuilder containerBuilder)
		{
			containerBuilder
				 .RegisterType<MessageBusSignalRClient>()
				 .As<MessageBusSignalRClient>()
				 .As<IMessageBusSignalRClientConfig>()
				 .As<IMessageBusSink>()
				 .SingleInstance();
		}

		public static void AddNoQSignalRClient(this ServiceCollection serviceCollection)
		{
			serviceCollection.TryAddSingleton<MessageBusSignalRClient>();
			serviceCollection.TryAddSingleton<IMessageBusSignalRClientConfig>(x => x.GetService<MessageBusSignalRClient>());
			serviceCollection.TryAddSingleton<IMessageBusSink>(x => x.GetService<MessageBusSignalRClient>());
		}

		public static Task StartNoQSignalRClient(this IMessageBusConfig config,
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
