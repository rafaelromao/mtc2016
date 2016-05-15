using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using MTC2016.DistributionList;
using MTC2016.Scheduler;
using Takenet.MessagingHub.Client.Listener;
using Takenet.MessagingHub.Client.Sender;

namespace MTC2016
{
    public sealed class Startup : IStartable, IDisposable
    {
        private readonly IMessagingHubSender _sender;
        private readonly ISchedulerExtension _schedulerExtension;
        private readonly IDistributionListExtension _distributionListExtension;
        private readonly ConsoleTraceListener _listener;

        public Startup(IMessagingHubSender sender, ISchedulerExtension schedulerExtension, IDistributionListExtension distributionListExtension)
        {
            _sender = sender;
            _schedulerExtension = schedulerExtension;
            _distributionListExtension = distributionListExtension;
            _listener = new ConsoleTraceListener(false);
            Trace.Listeners.Add(_listener);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await RecipientsRepository.EnsureMtc2016IsADistributionListAsync(_sender, cancellationToken);
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
