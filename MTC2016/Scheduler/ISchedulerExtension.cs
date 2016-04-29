using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;

namespace MTC2016.Scheduler
{
    public interface ISchedulerExtension
    {
        Task ScheduleAsync(Func<Task<IEnumerable<Identity>>> getRecipientsAsync, IEnumerable<ScheduledMessage> scheduledMessages, CancellationToken cancellationToken);
    }
}