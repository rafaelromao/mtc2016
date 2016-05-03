using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using MTC2016.ArtificialInteligence;
using MTC2016.Configuration;
using MTC2016.DistributionList;
using Takenet.MessagingHub.Client;
using Takenet.MessagingHub.Client.Listener;
using Takenet.MessagingHub.Client.Sender;

namespace MTC2016.Receivers
{
    public class UnsubscribeMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly IArtificialInteligenceExtension _artificialInteligenceExtension;
        private readonly IDistributionListExtension _distributionListExtension;
        private readonly Settings _settings;

        public UnsubscribeMessageReceiver(IMessagingHubSender sender, IArtificialInteligenceExtension artificialInteligenceExtension, 
            IDistributionListExtension distributionListExtension, Settings settings)
        {
            _sender = sender;
            _artificialInteligenceExtension = artificialInteligenceExtension;
            _distributionListExtension = distributionListExtension;
            _settings = settings;
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            if (!await _distributionListExtension.ContainsAsync(message.From, cancellationToken))
            {
                var answer = await _artificialInteligenceExtension.GetAnswerForAsync(_settings.NotSubscribed);
                await _sender.SendMessageAsync(answer, message.From, cancellationToken);
            }
            else if (await _distributionListExtension.RemoveAsync(message.From, cancellationToken))
            {
                var answer = await _artificialInteligenceExtension.GetAnswerForAsync(_settings.ConfirmSubscriptionCancellation);
                await _sender.SendMessageAsync(answer, message.From, cancellationToken);
            }
            else
            {
                var answer = await _artificialInteligenceExtension.GetAnswerForAsync(_settings.UnsubscriptionFailed);
                await _sender.SendMessageAsync(answer, message.From, cancellationToken);
            }
        }
    }
}
