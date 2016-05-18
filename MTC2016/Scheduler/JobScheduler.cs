using System;
using System.Threading.Tasks;
using Lime.Protocol;
using Takenet.MessagingHub.Client.Sender;

namespace MTC2016.Scheduler
{
    public class JobScheduler : IJobScheduler
    {
        private readonly IMessagingHubSender _sender;

        public JobScheduler(IMessagingHubSender sender)
        {
            _sender = sender;
        }

        public Task ScheduleAsync(Message message, DateTimeOffset time)
        {
#pragma warning disable 4014
            Task.Run(async () =>
#pragma warning restore 4014
            {
                while (true)
                {
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
    }
}