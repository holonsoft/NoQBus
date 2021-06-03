using System;

namespace holonsoft.NoQBus
{

   public partial class MessageBus : IMessageBusConfig, IMessageBusConfigure, IMessageBusConfigured
   {
      private bool _isConfigured;

      private void EnsureConfigured()
      {
         if (!_isConfigured)
            throw new NotSupportedException($"The {nameof(MessageBus)} is not configured!");
      }

      IMessageBusConfigure IMessageBusConfig.Configure()
         => _isConfigured ? throw new NotSupportedException($"More than one configuration for the {nameof(MessageBus)} is not supported") : this;

      IMessageBusConfigure IMessageBusConfigure.SetTimeoutTimeSpan(TimeSpan timeOutTimeSpan)
      {
         _timeOutTimeSpan = timeOutTimeSpan;
         return this;
      }

      IMessageBusConfigure IMessageBusConfigure.AsClient()
      {
         _isServer = false;
         return this;
      }

      IMessageBusConfigure IMessageBusConfigure.AsServer()
      {
         _isServer = true;
         return this;
      }

      IMessageBusConfigured IMessageBusConfigure.ConfigureForLocalUse()
      {
         return this;
      }

      IMessageBusConfigured IMessageBusConfigure.AddMessageSink(IMessageBusSink messageSink)
      {
         _messageSink = messageSink;
         return this;
      }

      void IMessageBusConfigured.Build()
      {
         _isConfigured = true;
      }
   }

}
