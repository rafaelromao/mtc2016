using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;

namespace MTC2016.Scheduler
{
    public interface ISchedulerExtension
    {
        Task ScheduleAsync(Func<Task<IEnumerable<Identity>>> recipients, IEnumerable<ScheduledMessage> scheduledMessages, CancellationToken cancellationToken);
        Task<IEnumerable<ScheduledMessage>> GetScheduledMessagesAsync(CancellationToken cancellationToken);
    }
}