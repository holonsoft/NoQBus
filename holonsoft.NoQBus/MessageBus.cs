using holonsoft.FluentConditions;
using holonsoft.NoQBus.Abstractions.Attributes;
using holonsoft.NoQBus.Abstractions.Contracts;
using holonsoft.NoQBus.Abstractions.Exceptions;
using holonsoft.NoQBus.Abstractions.Models;
using System.Collections.Concurrent;
using System.Reflection;

namespace holonsoft.NoQBus;
public partial class MessageBus
{
  private readonly ConcurrentDictionary<Type, ConcurrentDictionary<Guid, Func<IRequest, Task<IResponse>>>> _subscriptionsByType = new();
  private readonly ConcurrentDictionary<Guid, (Type Type, Func<IRequest, Task<IResponse>> ActionDelegate)> _subscriptionsByGuid = new();
  private readonly IMessageBusSink _messageSink;

  private TimeSpan _timeOutTimeSpan = TimeSpan.FromSeconds(300);
  private bool _throwIfNoReceiverSubscribed = true;

  public MessageBus(IMessageBusSink messageSink = null) => _messageSink = messageSink;

  public Task<Guid> Subscribe<TRequest>(Func<TRequest, Task> action) where TRequest : IRequest
  {
    action.Requires(nameof(action)).IsNotNull();

    return Subscribe<TRequest, VoidResponse>(x =>
      {
        _ = action(x).ConfigureAwait(false);
        return Task.FromResult(VoidResponse.Instance);
      });
  }

  public Task<Guid> Subscribe<TRequest, TResponse>(Func<TRequest, Task<TResponse>> action) where TRequest : IRequest
                                                                                           where TResponse : IResponse
  {
    action.Requires(nameof(action)).IsNotNull();

    var subscriptionId = Guid.NewGuid();

    async Task<IResponse> subscriptionNonGeneric(IRequest x) => await action((TRequest) x);

    _subscriptionsByGuid.GetOrAdd(subscriptionId, (typeof(TRequest), subscriptionNonGeneric));
    _subscriptionsByType.GetOrAdd(typeof(TRequest), x => new ConcurrentDictionary<Guid, Func<IRequest, Task<IResponse>>>())
                        .GetOrAdd(subscriptionId, subscriptionNonGeneric);

    return Task.FromResult(subscriptionId);
  }

  private Task CancelSubscription(Guid subscriptionId)
  {
    if (_subscriptionsByGuid.TryGetValue(subscriptionId, out var bySubscriptionId))
    {
      if (_subscriptionsByType.TryGetValue(bySubscriptionId.Type, out var bySubscriptionType))
      {
        bySubscriptionType.TryRemove(subscriptionId, out _);
      }

      _subscriptionsByGuid.TryRemove(subscriptionId, out _);
    }

    return Task.CompletedTask;
  }

  private async Task Publish(IRequest request)
     => await GetResponses(request);

  private async Task<IResponse[]> GetResponses(IRequest request, bool isRemoteCall = false)
  {
    if (!await HandleRequestFilterInternal(request))
    {
      return Array.Empty<IResponse>();
    }

    var responses = await GetResponsesInternal(request, isRemoteCall);

    return await HandleResponseFilterInternal(responses.Where(x => x is not VoidResponse));
  }

  private async Task<IResponse[]> GetResponsesInternal(IRequest request, bool isRemoteCall)
  {
    EnsureConfigured();

    request.Requires(nameof(request)).IsNotNull();

    var requestType = request.GetType();

    if (_subscriptionsByType.TryGetValue(requestType, out var subscriptions))
    {
      return await HandleLocalSubscriptions(request, subscriptions);
    }

    if (isRemoteCall)
    {
      return Array.Empty<IResponse>();
    }

    if (_messageSink != null)
    {
      if (requestType.GetCustomAttribute<DenyRemotingAttribute>(false) != null)
      {
        throw new RemotingDeniedException($"Messages of type {requestType.Name} must not be remoted!");
      }

      var result = await _messageSink.GetResponses(request);
      if (result.Length != 0)
      {
        return result;
      }
    }

    if (_throwIfNoReceiverSubscribed)
    {
      throw new NobodyListeningException($"Nobody is listening for messages of type {requestType.Name}");
    }

    return Array.Empty<IResponse>();
  }



  private async Task<IResponse[]> HandleLocalSubscriptions(IRequest request, ConcurrentDictionary<Guid, Func<IRequest, Task<IResponse>>> subscriptions)
  {
    request.Requires(nameof(request)).IsNotNull();
    subscriptions.Requires(nameof(request)).IsNotNull();

    if (subscriptions.Values.Count == 0)
    {
      return Array.Empty<IResponse>();
    }

    var resultingTasks =
               subscriptions.Values
                            .Select(x => x(request))
                            .ToArray();

    var timeoutTask = Task.Delay(_timeOutTimeSpan);
    var resultTask = Task.WhenAll(resultingTasks);
    await Task.WhenAny(timeoutTask, resultTask);
    if (!resultTask.IsCompleted && timeoutTask.IsCompleted)
    {
      throw new BusTimeOutException($"Timed out processing message of type {request.GetType().Name}");
    }

    return resultTask.Result;
  }
}
