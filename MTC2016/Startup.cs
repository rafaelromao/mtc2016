using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Takenet.MessagingHub.Client.Listener;

namespace MTC2016
{
    public sealed class Startup : IStartable, IDisposable
    {
        private readonly ConsoleTraceListener _listener;

        public Startup()
        {
            _listener = new ConsoleTraceListener(false);
            Trace.Listeners.Add(_listener);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _listener.Dispose();
        }
    }
}
