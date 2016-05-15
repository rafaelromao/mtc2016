using System;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;

namespace MTC2016.Scheduler
{
    public interface IJobScheduler : IDisposable
    {
        Task ScheduleAsync(Message message, DateTimeOffset time, CancellationToken cancellationToken);
    }
}
