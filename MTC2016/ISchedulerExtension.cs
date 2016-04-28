using System;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;

namespace MTC2016
{
    public interface ISchedulerExtension
    {
        Task ScheduleAsync(DateTimeOffset reminderTime, string message, CancellationToken cancellationToken, params Identity[] recipients);
    }
}