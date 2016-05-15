using System;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Takenet.Iris.Application.Scheduler.Resources;
using Takenet.MessagingHub.Client.Sender;

namespace MTC2016.Scheduler
{
    public class JobScheduler : IJobScheduler
    {
        internal static Node Scheduler => Node.Parse("postmaster@scheduler.msging.net");

        private readonly IMessagingHubSender _sender;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public JobScheduler(IMessagingHubSender sender)
        {
            _sender = sender;
        }

        public Task ScheduleAsync(Message message, DateTimeOffset time, CancellationToken cancellationToken)
        {
            var command = new Command(Guid.NewGuid().ToString())
            {
                To = Scheduler,
                Method = CommandMethod.Set,
                Uri = new LimeUri("/schedules"),
                Resource = new Schedule
                {
                    Message = message,
                    When = time
                }
            };
            return _sender.SendCommandAsync(command, cancellationToken);

        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}