using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
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
				 .AsSelf()
				 .As<IMessageBusSink>()
				 .As<IMessageBusSignalRHostConfig>()
				 .SingleInstance();

			containerBuilder
				 .RegisterType<MessageBusSignalRHubStateStore>()
				 .AsSelf()
				 .SingleInstance();
		}

		public static void AddNoQSignalRHost(this IServiceCollection serviceCollection)
		{
			serviceCollection.TryAddSingleton<MessageBusSignalRHost>();
			serviceCollection.TryAddSingleton<IMessageBusSignalRHostConfig>(x => x.GetService<MessageBusSignalRHost>());
			serviceCollection.TryAddSingleton<IMessageBusSink>(x => x.GetService<MessageBusSignalRHost>());
			serviceCollection.TryAddSingleton<MessageBusSignalRHubStateStore>();
		}

		public static void UseNoQSignalRHost(this IApplicationBuilder applicationBuilder)
		{
			var host = applicationBuilder.ApplicationServices.GetRequiredService<MessageBusSignalRHost>();
			var context = applicationBuilder.ApplicationServices.GetRequiredService<IHubContext<MessageBusSignalRHub, IMessageBusSignalRClient>>();
			host.SetHubContext(context);
		}

		public static async Task StartNoQSignalRHostOnExistingAspNetCoreHost(
			this IHost host,
			Action<IMessageBusSignalRHostConfig> configure = null,
			CancellationToken cancellationToken = default)
		{
			var messágeBusSignalRHost = host.Services.GetRequiredService<MessageBusSignalRHost>();
			messágeBusSignalRHost.ExistingAspNetCoreHost = true;
			var messageBusConfig = host.Services.GetRequiredService<IMessageBusConfig>();

			await StartNoQSignalRHost(messageBusConfig, configure, cancellationToken);
		}

		public static void MapNoQSignalRHost(this IEndpointRouteBuilder endpointRouteBuilder, string path)
		{
			endpointRouteBuilder.MapHub<MessageBusSignalRHub>(path);
		}

		public static Task StartNoQSignalRHost(this IMessageBusConfig config,
																					Action<IMessageBusSignalRHostConfig> configure = default,
																					CancellationToken cancellationToken = default)
		{
			return
				config
					.Configure()
					.DontThrowIfNoReceiverSubscribed()
					.ConfigureSink(x => configure?.Invoke((IMessageBusSignalRHostConfig) x))
					.StartAsync(cancellationToken);
		}

		public static Task StartNoQSignalRHost(this ILifetimeScope lifetimeScope,
																						Action<IMessageBusSignalRHostConfig> configure = default,
																					 CancellationToken cancellationToken = default)
			=> lifetimeScope.Resolve<IMessageBusConfig>().StartNoQSignalRHost(configure, cancellationToken);


		public static Task StartNoQSignalRHost(this ServiceProvider serviceProvider,
																					 Action<IMessageBusSignalRHostConfig> configure = default,
																					 CancellationToken cancellationToken = default)
			=> serviceProvider.GetRequiredService<IMessageBusConfig>().StartNoQSignalRHost(configure, cancellationToken);
	}
}
