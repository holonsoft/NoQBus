﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace holonsoft.NoQBus
{

	public partial class MessageBus
	{
		private readonly ConcurrentDictionary<Type, ConcurrentDictionary<Guid, Func<IRequest, Task<IResponse>>>> _subscriptionsByType = new();
		private readonly ConcurrentDictionary<Guid, (Type Type, Func<IRequest, Task<IResponse>> ActionDelegate)> _subscriptionsByGuid = new();
		private readonly IMessageBusSink _messageSink;

		private TimeSpan _timeOutTimeSpan = TimeSpan.FromSeconds(300);
		private bool _isServer = true;

		public MessageBus(IMessageBusSink messageSink = null)
		{
			_messageSink = messageSink;
		}

		public Task<Guid> Subscribe<TRequest>(Func<TRequest, Task> action) where TRequest : IRequest
			 => Subscribe<TRequest, VoidResponse>(x =>
			 {
				 _ = action(x).ConfigureAwait(false);
				 return Task.FromResult(VoidResponse.Instance);
			 });

		public Task<Guid> Subscribe<TRequest, TResponse>(Func<TRequest, Task<TResponse>> action) where TRequest : IRequest
																																														 where TResponse : IResponse
		{
			Guid subscriptionId = Guid.NewGuid();

			async Task<IResponse> subscriptionNonGeneric(IRequest x) => await action((TRequest) x);

			_subscriptionsByGuid.GetOrAdd(subscriptionId, (typeof(TRequest), (Func<IRequest, Task<IResponse>>) subscriptionNonGeneric));
			_subscriptionsByType.GetOrAdd(typeof(TRequest), x => new ConcurrentDictionary<Guid, Func<IRequest, Task<IResponse>>>())
													.GetOrAdd(subscriptionId, subscriptionNonGeneric);

			return Task.FromResult(subscriptionId);
		}

		private Task CancelSubscription(Guid subscriptionId)
		{
			if (_subscriptionsByGuid.TryGetValue(subscriptionId, out var bySubscriptionId))
			{
				if (_subscriptionsByType.TryGetValue(bySubscriptionId.Type, out var bySubscriptionType))
					bySubscriptionType.TryRemove(subscriptionId, out _);
				_subscriptionsByGuid.TryRemove(subscriptionId, out _);
			}

			return Task.CompletedTask;
		}

		private async Task Publish(IRequest request)
			 => await GetResponses(request);

		private async Task<IResponse[]> GetResponses(IRequest request, bool isRemoteCall = false)
		{
			EnsureConfigured();

			Type requestType = request.GetType();

			if (_subscriptionsByType.TryGetValue(requestType, out var subscriptions))
				return await HandleLocalSubscriptions(request, subscriptions);

			if (isRemoteCall)
				return Array.Empty<IResponse>();

			if (_messageSink != null)
			{
				if (requestType.GetCustomAttribute<DenyRemotingAttribute>(false) != null)
					throw new RemotingDeniedException($"Messages of type {requestType.Name} must not be remoted!");

				var result = await _messageSink.GetResponses(request);
				if (result.Length != 0)
					return result;
			}

			if (_isServer)
				return Array.Empty<IResponse>();

			throw new NobodyListeningException($"Nobody is listening for messages of type {requestType.Name}");
		}

		private async Task<IResponse[]> HandleLocalSubscriptions(IRequest request, ConcurrentDictionary<Guid, Func<IRequest, Task<IResponse>>> subscriptions)
		{
			var resultingTasks =
								 subscriptions.Values
															.Select(x => x(request))
															.ToArray();

			Task timeoutTask = Task.Delay(_timeOutTimeSpan);
			Task<IResponse[]> resultTask = Task.WhenAll(resultingTasks);
			await Task.WhenAny(timeoutTask, resultTask);
			if (timeoutTask.IsCompleted)
				throw new BusTimeOutException($"Timed out processing message of type {request.GetType().Name}");

			return resultTask.Result;
		}
	}
}