namespace holonsoft.NoQBus.SignalR.Host;

public partial class MessageBusSignalRHost : IMessageBusSignalRHostConfig
{
  IMessageBusSignalRHostConfig IMessageBusSignalRHostConfig.UseUrl(string url)
  {
    Url = url;
    return this;
  }
}
