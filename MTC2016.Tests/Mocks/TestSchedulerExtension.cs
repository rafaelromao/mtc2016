using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lime.Messaging.Contents;
using Lime.Protocol;
using MTC2016.Configuration;
using Takenet.MessagingHub.Client.Sender;

namespace MTC2016.Scheduler
{
    public sealed class TestSchedulerExtension : ISchedulerExtension
    {
        public Task ScheduleAsync(Func<Task<IEnumerable<Identity>>> getRecipientsAsync, IEnumerable<ScheduledMessage> scheduledMessages, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}