# NoQBus
An async, fast and slim message bus for transparent inproc / outbound messaging

It has
- no queue!
- fully async processing
- parallelization => every consumer runs it's own task/thread!
- transparent client / server functionality
- immutable records as messages
- no serialization if a message stays on one side!

## Samples

### Subscription with no response:
``` c#
private async Task SubscriberFunc(MyMessage message) {
   //do sth with your message while running on your own task/thread
}

await _messageBus.Subscribe<MyMessage>(SubscriberFunc);
```

### Subscription with response:
``` c#
private async Task<MyResponse> SubscriberFunc(MyRequest message) {
   //do sth with your message while running on your own task/thread
   return new MyResponse(someResult);
}

await _messageBus.Subscribe<MyRequest, MyResponse>(SubscriberFunc);
```

### Publish fire and forget message:

``` c#
await _messageBus.Publish(new MyMessage());
```


### Publish and request some responses:
As many can subscribe to the same messages you can get multiple responses.
``` c#
MyResponse[] responses = await _messageBus.GetResponses<MyResponse>(new MyRequest());
```

### Sample message:
``` c#
public record MyMessage : MessageBase {
  public string OneOfMyProps { get; init; }
}
```

### Sample request:
``` c#
public record MyRequest : RequestBase {
  public string OneOfMyProps { get; init; }
}
```

### Sample response:
``` c#
public record MyResponse : ResponseBase {
  public string OneOfMyProps { get; init; }
}
```

### Request and response with known response/request type
``` c#
public record MyRequest : RequestBase<MyResponse> {
  public string OneOfMyProps { get; init; }
}

public record MyResponse : ResponseBase<MyRequest> {
  public string OneOfMyProps { get; init; }
}
```

### Starting the Messagebus
#### Only local

``` c#
diContainerBuilder.AddNoQMessageBus(); //or servicecollection
//after building the container:
diContainer.Resolve<IMessageBusConfig>().StartLocalNoQMessageBus();
//from now on you can use:
_messageBus = diContainer.Resolve<IMessageBus>();
```


#### SignalR server
``` c#
diContainerBuilder.AddNoQMessageBus(); //or servicecollection
diContainerBuilder.AddNoQSignalRHost(); //or servicecollection
//after building the container:
diContainer.Resolve<IMessageBusConfig>().StartNoQSignalRHost(x => x.UseUrl("http://localhost:5001"));
//from now on you can use:
_messageBus = diContainer.Resolve<IMessageBus>();
```

#### SignalR client
``` c#
diContainerBuilder.AddNoQMessageBus(); //or servicecollection
diContainerBuilder.AddNoQSignalRClient(); //or servicecollection
//after building the container:
diContainer.Resolve<IMessageBusConfig>().StartNoQSignalRClient(x => x.UseUrl("http://localhost:5001"));
//from now on you can use:
_messageBus = diContainer.Resolve<IMessageBus>();
```
