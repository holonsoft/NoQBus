namespace holonsoft.NoQBus.SignalR.Client;

public partial class MessageBusSignalRClient : IMessageBusSignalRClientConfig
{
  IMessageBusSignalRClientConfig IMessageBusSignalRClientConfig.UseUrl(string url)
  {
    Url = url;
    return this;
  }

  IMessageBusSignalRClientConfig IMessageBusSignalRClientConfig.SetInitialRetryCount(int initialRetryCount)
  {
    InitialRetryCount = initialRetryCount;
    return this;
  }

  IMessageBusSignalRClientConfig IMessageBusSignalRClientConfig.SetRetryDelay(TimeSpan retryDelay)
  {
    RetryDelay = retryDelay;
    return this;
  }
}
