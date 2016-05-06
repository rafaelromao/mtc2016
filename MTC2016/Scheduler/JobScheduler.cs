using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTC2016.Scheduler
{
    public class JobScheduler : IJobScheduler
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public Task ScheduleAsync(Action action, DateTimeOffset time, CancellationToken cancellationToken)
        {
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);

            Task.Run(async () =>
            {
                while (true)
                {
                    if (cancellationTokenSource.IsCancellationRequested)
                        break;
                    if (DateTime.Now > time)
                    {
                        action();
                        break;
                    }
                    await Task.Delay(TimeSpan.FromMinutes(1), cancellationTokenSource.Token);
                }
            }, cancellationTokenSource.Token);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}