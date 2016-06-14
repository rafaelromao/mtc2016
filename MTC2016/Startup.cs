using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MTC2016.Scheduler;
using Takenet.MessagingHub.Client.Listener;

namespace MTC2016
{
    public sealed class Startup : IStartable, IDisposable
    {
        private readonly ISchedulerExtension _schedulerExtension;
        private readonly ConsoleTraceListener _listener;

        public Startup(ISchedulerExtension schedulerExtension)
        {
            _schedulerExtension = schedulerExtension;
            _listener = new ConsoleTraceListener(false);
            Trace.Listeners.Add(_listener);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Connected!");
            await ScheduleScheduledMessagesAsync();
        }

        private async Task ScheduleScheduledMessagesAsync()
        {
            await _schedulerExtension.UpdateSchedulesAsync();
        }

        public void Dispose()
        {
            _listener.Dispose();
        }
    }
}
