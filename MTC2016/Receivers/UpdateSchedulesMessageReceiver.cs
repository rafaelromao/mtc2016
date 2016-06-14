using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using MTC2016.Scheduler;
using Takenet.MessagingHub.Client.Listener;

namespace MTC2016.Receivers
{
    public class UpdateSchedulesMessageReceiver : IMessageReceiver
    {
        private readonly ISchedulerExtension _schedulerExtension;

        public UpdateSchedulesMessageReceiver(ISchedulerExtension schedulerExtension)
        {
            _schedulerExtension = schedulerExtension;
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            await _schedulerExtension.UpdateSchedulesAsync();
        }
    }
}
