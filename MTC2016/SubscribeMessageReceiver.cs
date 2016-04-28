using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Takenet.MessagingHub.Client;
using Takenet.MessagingHub.Client.Listener;
using Takenet.MessagingHub.Client.Sender;

namespace MTC2016
{
    public class SubscribeMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly IDistributionListExtension _distributionListExtension;
        private readonly ISchedulerExtension _schedulerExtension;
        private readonly Settings _settings;

        public SubscribeMessageReceiver(IMessagingHubSender sender, IDistributionListExtension distributionListExtension, ISchedulerExtension schedulerExtension, Settings settings)
        {
            _sender = sender;
            _distributionListExtension = distributionListExtension;
            _schedulerExtension = schedulerExtension;
            _settings = settings;
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            if (await _distributionListExtension.AddAsync(message.From.ToIdentity()))
            {
                await ScheduleMessagesForNewRecipientAsync(message.From, cancellationToken);
                await _sender.SendMessageAsync(_settings.Messages.ConfirmSubscription, message.From, cancellationToken);
            }
            else
            {
                await _sender.SendMessageAsync(_settings.Messages.SubscriptionFailed, message.From, cancellationToken);
            }
        }

        private async Task ScheduleMessagesForNewRecipientAsync(Identity recipient, CancellationToken cancellationToken)
        {
            foreach (var reminder in _settings.Reminders)
            {
                await _schedulerExtension.ScheduleAsync(reminder.Time, reminder.Message, cancellationToken, recipient);
            }
        }
    }
}
