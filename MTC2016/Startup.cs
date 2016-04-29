using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using MTC2016.Configuration;
using MTC2016.DistributionList;
using MTC2016.Scheduler;
using Takenet.MessagingHub.Client.Listener;

namespace MTC2016
{
    public sealed class Startup : IStartable, IDisposable
    {
        private readonly ISchedulerExtension _schedulerExtension;
        private readonly IDistributionListExtension _distributionListExtension;
        private readonly Settings _settings;
        private readonly ConsoleTraceListener _listener;

        public Startup(ISchedulerExtension schedulerExtension, IDistributionListExtension distributionListExtension, Settings settings)
        {
            _schedulerExtension = schedulerExtension;
            _distributionListExtension = distributionListExtension;
            _settings = settings;
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
                GetRecipientsAsync(cancellationToken),
                _settings.ScheduledMessages,
                cancellationToken);
        }

        private Func<Task<IEnumerable<Identity>>> GetRecipientsAsync(CancellationToken cancellationToken)
        {
            return async () => await _distributionListExtension.GetRecipientsAsync(cancellationToken);
        }

        public void Dispose()
        {
            _listener.Dispose();
        }
    }
}
