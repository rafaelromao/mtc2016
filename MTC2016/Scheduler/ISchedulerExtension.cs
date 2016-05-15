using System.Threading;
using System.Threading.Tasks;

namespace MTC2016.Scheduler
{
    public interface ISchedulerExtension
    {
        Task UpdateSchedulesAsync(CancellationToken cancellationToken);
    }
}