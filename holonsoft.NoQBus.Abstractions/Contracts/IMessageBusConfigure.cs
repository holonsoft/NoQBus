using System;

namespace holonsoft.NoQBus
{

   public interface IMessageBusConfigure
   {
      public IMessageBusConfigure SetTimeoutTimeSpan(TimeSpan timeOutTimeSpan);
      public IMessageBusConfigure AsClient();
      public IMessageBusConfigure AsServer();
      public IMessageBusConfigured AddMessageSink(IMessageBusSink messageSink);
      public IMessageBusConfigured ConfigureForLocalUse();
   }
}