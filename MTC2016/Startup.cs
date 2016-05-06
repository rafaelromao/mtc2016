using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MTC2016.DistributionList;
using MTC2016.Scheduler;
using Takenet.MessagingHub.Client.Listener;

namespace MTC2016
{
    public sealed class Startup : IStartable, IDisposable
    {
        private readonly ISchedulerExtension _schedulerExtension;
        private readonly IDistributionListExtension _distributionListExtension;
        private readonly ConsoleTraceListener _listener;

        public Startup(ISchedulerExtension schedulerExtension, IDistributionListExtension distributionListExtension)
        {
            _schedulerExtension = schedulerExtension;
            _distributionListExtension = distributionListExtension;
            _listener = new ConsoleTraceListener(false);
            Trace.Listeners.Add(_listener);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await ScheduleScheduledMessagesAsync(cancellationToken);
        }

        private async Task ScheduleScheduledMessagesAsync(CancellationToken cancellationToken)
        {
            await _schedulerExtension.ScheduleAsync(
                async () => await _distributionListExtension.GetRecipientsAsync(cancellationToken),
                await _schedulerExtension.GetScheduledMessagesAsync(cancellationToken),
                cancellationToken);
        }

        public void Dispose()
        {
            _listener.Dispose();
        }
    }
}
