using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using MTC2016.Configuration;
using MTC2016.DistributionList;
using Takenet.MessagingHub.Client;
using Takenet.MessagingHub.Client.Listener;
using Takenet.MessagingHub.Client.Sender;

namespace MTC2016.Receivers
{
    public class SubscribeMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly IDistributionListExtension _distributionListExtension;
        private readonly Settings _settings;

        public SubscribeMessageReceiver(IMessagingHubSender sender, IDistributionListExtension distributionListExtension, Settings settings)
        {
            _sender = sender;
            _distributionListExtension = distributionListExtension;
            _settings = settings;
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            if (await _distributionListExtension.ContainsAsync(message.From, cancellationToken))
            {
                await _sender.SendMessageAsync(_settings.Messages.AlreadySubscribed, message.From, cancellationToken);
            }
            else if (await _distributionListExtension.AddAsync(message.From, cancellationToken))
            {
                await _sender.SendMessageAsync(_settings.Messages.ConfirmSubscription, message.From, cancellationToken);
            }
            else
            {
                await _sender.SendMessageAsync(_settings.Messages.SubscriptionFailed, message.From, cancellationToken);
            }
        }
    }
}
