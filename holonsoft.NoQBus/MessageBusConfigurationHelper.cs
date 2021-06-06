using Autofac;
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
				 .SingleInstance();
		}

		public static void AddNoQMessageBus(this ServiceCollection serviceCollection)
		{
			serviceCollection.TryAddSingleton<MessageBus>();
			serviceCollection.TryAddSingleton<IMessageBus>(x => x.GetService<MessageBus>());
			serviceCollection.TryAddSingleton<IMessageBusConfig>(x => x.GetService<MessageBus>());
			serviceCollection.TryAddSingleton<IRemoteMessageBus>(x => x.GetService<MessageBus>());
		}

		public static Task StartLocalNoQMessageBus(this IMessageBusConfig config,
																							 CancellationToken cancellationToken = default)
		{
			return
				config
					.Configure()
					.AsServer()
					.StartAsync(cancellationToken);
		}
	}
}
