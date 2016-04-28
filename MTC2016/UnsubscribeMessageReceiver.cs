using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Takenet.MessagingHub.Client;
using Takenet.MessagingHub.Client.Listener;
using Takenet.MessagingHub.Client.Sender;

namespace MTC2016
{
    public class UnsubscribeMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly IDistributionListExtension _distributionListExtension;
        private readonly Settings _settings;

        public UnsubscribeMessageReceiver(IMessagingHubSender sender, IDistributionListExtension distributionListExtension, Settings settings)
        {
            _sender = sender;
            _distributionListExtension = distributionListExtension;
            _settings = settings;
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            if (!await _distributionListExtension.ContainsAsync(message.From.ToIdentity()))
            {
                await _sender.SendMessageAsync(_settings.Messages.NotSubscribed, message.From, cancellationToken);
            }
            else if (await _distributionListExtension.RemoveAsync(message.From.ToIdentity()))
            {
                await _sender.SendMessageAsync(_settings.Messages.ConfirmSubscriptionCancellation, message.From, cancellationToken);
            }
            else
            {
                await _sender.SendMessageAsync(_settings.Messages.SubscriptionFailed, message.From, cancellationToken);
            }
        }
    }
}
