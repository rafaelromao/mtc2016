using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Takenet.MessagingHub.Client.Sender;

namespace MTC2016.Scheduler
{
    public sealed class JobScheduler : IJobScheduler
    {
        private readonly IMessagingHubSender _sender;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public JobScheduler(IMessagingHubSender sender)
        {
            _sender = sender;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public Task ScheduleAsync(Message message, DateTimeOffset time)
        {
#pragma warning disable 4014
            Task.Run(async () =>
#pragma warning restore 4014
            {
                while (true)
                {
                    if (_cancellationTokenSource.IsCancellationRequested)
                        break;

                    if (DateTime.Now >= time)
                    {
                        await _sender.SendMessageAsync(message);
                        break;
                    }

                    await Task.Delay(TimeSpan.FromMinutes(1));
                }
            });
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}