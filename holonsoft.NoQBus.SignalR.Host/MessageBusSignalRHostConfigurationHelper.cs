using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace holonsoft.NoQBus.SignalR.Host
{
	public static class MessageBusSignalRHostConfigurationHelper
	{
		public static void AddNoQSignalRHost(this ContainerBuilder containerBuilder)
		{
			containerBuilder
				 .RegisterType<MessageBusSignalRHost>()
				 .As<MessageBusSignalRHost>()
				 .As<IMessageBusSink>()
				 .As<IMessageBusSignalRHostConfig>()
				 .SingleInstance();
		}

		public static void AddNoQSignalRHost(this ServiceCollection serviceCollection)
		{
			serviceCollection.TryAddSingleton<MessageBusSignalRHost>();
			serviceCollection.TryAddSingleton<IMessageBusSignalRHostConfig>(x => x.GetService<MessageBusSignalRHost>());
			serviceCollection.TryAddSingleton<IMessageBusSink>(x => x.GetService<MessageBusSignalRHost>());
		}

		public static Task StartNoQSignalRHost(this IMessageBusConfig config,
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
