using Autofac;
using holonsoft.NoQBus.Abstractions.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace holonsoft.NoQBus
{
	public static class MessageBusSignalRHostConfigurationHelper
	{
		public static void AddNoQMessageBus(this ContainerBuilder containerBuilder)
		{
			containerBuilder
				 .RegisterType<MessageBus>()
				 .As<IMessageBus>()
				 .As<IMessageBusConfig>()
				 .As<IRemoteMessageBus>()
				 .As<IMessageBusFiltering>()
				 .SingleInstance();
		}

		public static void AddNoQMessageBus(this IServiceCollection serviceCollection)
		{
			serviceCollection.TryAddSingleton<MessageBus>();
			serviceCollection.TryAddSingleton<IMessageBus>(x => x.GetService<MessageBus>());
			serviceCollection.TryAddSingleton<IMessageBusConfig>(x => x.GetService<MessageBus>());
			serviceCollection.TryAddSingleton<IRemoteMessageBus>(x => x.GetService<MessageBus>());
			serviceCollection.TryAddSingleton<IMessageBusFiltering>(x => x.GetService<MessageBus>());
		}

		public static Task StartLocalNoQMessageBus(this IMessageBusConfig config,
																							 CancellationToken cancellationToken = default)
		{
			return
				config
					.Configure()
					.DontThrowIfNoReceiverSubscribed()
					.StartAsync(cancellationToken);
		}

		public static Task StartLocalNoQMessageBus(this ILifetimeScope lifetimeScope,
																							 CancellationToken cancellationToken = default)
			=> lifetimeScope.Resolve<IMessageBusConfig>().StartLocalNoQMessageBus(cancellationToken);

		public static Task StartLocalNoQMessageBus(this ServiceProvider serviceProvider,
																							 CancellationToken cancellationToken = default)
			=> serviceProvider.GetRequiredService<IMessageBusConfig>().StartLocalNoQMessageBus(cancellationToken);
	}
}
