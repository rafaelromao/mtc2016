using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTC2016.Scheduler
{
    public interface IJobScheduler : IDisposable
    {
        Task ScheduleAsync(Action action, DateTimeOffset time, CancellationToken cancellationToken);
    }
}
