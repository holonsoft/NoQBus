using holonsoft.FluentConditions;
using holonsoft.NoQBus.Abstractions.Contracts;
using System.Collections.Concurrent;

namespace holonsoft.NoQBus;
public partial class MessageBus
{
  private readonly ConcurrentDictionary<Type, ConcurrentDictionary<Guid, Func<IRequest, Task<bool>>>> _requestFilterByType = new();
  private readonly ConcurrentDictionary<Guid, (Type Type, Func<IRequest, Task<bool>> filterDelegate)> _requestFilterByGuid = new();

  private readonly ConcurrentDictionary<Type, ConcurrentDictionary<Guid, Func<IEnumerable<IResponse>, Task<IEnumerable<IResponse>>>>> _responseFilterByType = new();
  private readonly ConcurrentDictionary<Guid, (Type Type, Func<IEnumerable<IResponse>, Task<IEnumerable<IResponse>>> filterDelegate)> _responseFilterByGuid = new();

  private Task<Guid> AddRequestFilter<TRequest>(Func<TRequest, Task<bool>> filter) where TRequest : IRequest
  {
    filter.Requires(nameof(filter)).IsNotNull();

    var requestFilterId = Guid.NewGuid();

    async Task<bool> filterNonGeneric(IRequest x) => await filter((TRequest) x);

    _requestFilterByGuid.GetOrAdd(requestFilterId, (typeof(TRequest), filterNonGeneric));
    _requestFilterByType.GetOrAdd(typeof(TRequest), x => new ConcurrentDictionary<Guid, Func<IRequest, Task<bool>>>())
                        .GetOrAdd(requestFilterId, filterNonGeneric);

    return Task.FromResult(requestFilterId);
  }

  private Task RemoveRequestFilter(Guid requestFilterId)
  {
    if (_requestFilterByGuid.TryGetValue(requestFilterId, out var byRequestFilterId))
    {
      if (_requestFilterByType.TryGetValue(byRequestFilterId.Type, out var byRequestFilterType))
      {
        byRequestFilterType.TryRemove(requestFilterId, out _);
      }

      _requestFilterByGuid.TryRemove(requestFilterId, out _);
    }

    return Task.CompletedTask;
  }

  private async Task<bool> HandleRequestFilterInternal(IRequest request)
  {
    if (_requestFilterByType.TryGetValue(request.GetType(), out var requestFilters))
    {
      if (requestFilters.Values.Count == 0)
      {
        return true;
      }

      var resultingTasks =
         requestFilters
          .Values
          .Select(x => x(request))
          .ToArray();

      var results = await Task.WhenAll(resultingTasks);

      return results.All(x => x);
    }

    return true;
  }

  private async Task<IResponse[]> HandleResponseFilterInternal(IEnumerable<IResponse> responses)
  {
    var resultingTasks =
      responses.GroupBy(x => x.GetType())
               .Select(x => FilterResponsesOfOneType(x.Key, x));

    var results = await Task.WhenAll(resultingTasks);

    return results.SelectMany(x => x).ToArray();

    async Task<IEnumerable<IResponse>> FilterResponsesOfOneType(Type responseType, IEnumerable<IResponse> responsesOfType)
    {
      if (_responseFilterByType.TryGetValue(responseType, out var responseFilters))
      {
        if (responseFilters.Values.Count == 0)
        {
          return responsesOfType;
        }

        var resultingTasks =
           responseFilters
            .Values
            .Select(x => x(responsesOfType))
            .ToArray();

        var results = await Task.WhenAll(resultingTasks);

        return results.Aggregate(responsesOfType, (x, y) => x.Intersect(y));
      }

      return responsesOfType;
    }
  }

  private Task<Guid> AddResponseFilter<TResponse>(Func<IEnumerable<TResponse>, Task<IEnumerable<TResponse>>> filter) where TResponse : IResponse
  {
    filter.Requires(nameof(filter)).IsNotNull();

    var responseFilterId = Guid.NewGuid();

    async Task<IEnumerable<IResponse>> filterNonGeneric(IEnumerable<IResponse> x) => (await filter(x.Cast<TResponse>())).Cast<IResponse>();

    _responseFilterByGuid.GetOrAdd(responseFilterId, (typeof(TResponse), filterNonGeneric));
    _responseFilterByType.GetOrAdd(typeof(TResponse), x => new ConcurrentDictionary<Guid, Func<IEnumerable<IResponse>, Task<IEnumerable<IResponse>>>>())
                        .GetOrAdd(responseFilterId, filterNonGeneric);

    return Task.FromResult(responseFilterId);
  }

  private Task RemoveResponseFilter(Guid responseFilterId)
  {
    if (_responseFilterByGuid.TryGetValue(responseFilterId, out var byResponseFilterId))
    {
      if (_responseFilterByType.TryGetValue(byResponseFilterId.Type, out var byResponseFilterType))
      {
        byResponseFilterType.TryRemove(responseFilterId, out _);
      }

      _responseFilterByGuid.TryRemove(responseFilterId, out _);
    }

    return Task.CompletedTask;
  }
}
