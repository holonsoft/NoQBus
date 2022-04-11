using holonsoft.NoQBus.Remoting.Models;
using System.Collections.Concurrent;

namespace holonsoft.NoQBus.SignalR.Host;

public class MessageBusSignalRHubStateStore
{
  private readonly HashSet<string> _connections = new();

  public void ClientConnected(string connectionId)
  {
    lock (_connections)
    {
      _connections.Add(connectionId);
    }
  }

  public void ClientDisconnected(string connectionId)
  {
    lock (_connections)
    {
      _connections.Remove(connectionId);

      if (_waitingPerConnectionId.TryGetValue(connectionId, out var waitingFor))
      {
        lock (waitingFor)
        {
          foreach (var oneWaiting in waitingFor)
          {
            if (_waitingForResponses.TryGetValue(oneWaiting, out var waitingRequest))
            {
              CheckResponsesComplete(oneWaiting, connectionId, waitingRequest.TaskCompletionSource, waitingRequest.WaitingForRespondingConnections, waitingRequest.Responses);
            }
          }
        }
      }
    }
  }

  public string[] GetConnections()
  {
    lock (_connections)
    {
      return _connections.ToArray();
    }
  }

  private readonly ConcurrentDictionary<Guid, (TaskCompletionSource<SinkTransportDataResponse> TaskCompletionSource,
                                               HashSet<string> WaitingForRespondingConnections,
                                               HashSet<SinkTransportDataResponse> Responses)> _waitingForResponses = new();

  private readonly ConcurrentDictionary<string, HashSet<Guid>> _waitingPerConnectionId = new();


  public Task<SinkTransportDataResponse> AwaitResultForRequest(SinkTransportDataRequest request, string[] connections)
  {
    TaskCompletionSource<SinkTransportDataResponse> taskCompletionSource = new();
    lock (_connections)
    {
      var waitingForRespondingConnections = new HashSet<string>(connections.Where(_connections.Contains));
      lock (waitingForRespondingConnections)
      {
        _waitingForResponses[request.RequestIdentifier] = (taskCompletionSource,
                                                           waitingForRespondingConnections,
                                                           new HashSet<SinkTransportDataResponse>());

        foreach (var waitingForConnectionId in waitingForRespondingConnections)
        {
          var waitingForConnection = _waitingPerConnectionId.GetOrAdd(waitingForConnectionId, x => new HashSet<Guid>());
          lock (waitingForConnection)
          {
            waitingForConnection.Add(request.RequestIdentifier);
          }
        }
      }
    }
    return taskCompletionSource.Task;
  }

  private void CheckResponsesComplete(Guid requestIdentifier,
                                      string connectionId,
                                      TaskCompletionSource<SinkTransportDataResponse> taskCompletionSource,
                                      HashSet<string> waitingForRespondingConnections,
                                      HashSet<SinkTransportDataResponse> responses)
  {
    lock (waitingForRespondingConnections)
    {
      if (waitingForRespondingConnections.Remove(connectionId))
      {
        if (waitingForRespondingConnections.Count == 0)
        {
          lock (responses)
          {
            _waitingForResponses.Remove(requestIdentifier, out _);
            taskCompletionSource.SetResult(new SinkTransportDataResponse(requestIdentifier, responses.SelectMany(x => x.ResponseEntries).ToArray()));
          }
        }
      }
    }
  }

  public void ReceivedResponse(string connectionId, SinkTransportDataResponse response)
  {
    (var taskCompletionSource,
       var waitingForRespondingConnections,
       var responses) = _waitingForResponses[response.RequestIdentifier];

    lock (responses)
    {
      responses.Add(response);
    }
    CheckResponsesComplete(response.RequestIdentifier, connectionId, taskCompletionSource, waitingForRespondingConnections, responses);
  }
}
