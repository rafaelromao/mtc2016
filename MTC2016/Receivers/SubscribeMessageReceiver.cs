using System;
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
    public class SubscribeMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly IArtificialInteligenceExtension _artificialInteligenceExtension;
        private readonly IDistributionListExtension _distributionListExtension;
        private readonly Settings _settings;
        private readonly string _defaultAnswer;

        public SubscribeMessageReceiver(IMessagingHubSender sender, IArtificialInteligenceExtension artificialInteligenceExtension,
            IDistributionListExtension distributionListExtension, Settings settings)
        {
            _sender = sender;
            _artificialInteligenceExtension = artificialInteligenceExtension;
            _distributionListExtension = distributionListExtension;
            _settings = settings;
            try
            {
                _defaultAnswer = _artificialInteligenceExtension.GetAnswerAsync(_settings.CouldNotUnderstand).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception when querying for {_settings.CouldNotUnderstand}: {e}");
                _defaultAnswer = _settings.GeneralError;
            }
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            try
            {
                if (await _distributionListExtension.ContainsAsync(message.From, cancellationToken))
                {
                    var answer = await _artificialInteligenceExtension.GetAnswerAsync(_settings.AlreadySubscribed);
                    await _sender.SendMessageAsync(answer, message.From, cancellationToken);
                }
                else if (await _distributionListExtension.AddAsync(message.From, cancellationToken))
                {
                    var answer = await _artificialInteligenceExtension.GetAnswerAsync(_settings.ConfirmSubscription);
                    await _sender.SendMessageAsync(answer, message.From, cancellationToken);
                }
                else
                {
                    var answer = await _artificialInteligenceExtension.GetAnswerAsync(_settings.SubscriptionFailed);
                    await _sender.SendMessageAsync(answer, message.From, cancellationToken);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception when trying to subscribe: {e}");
                try
                {
                    await _sender.SendMessageAsync(_defaultAnswer, message.From, cancellationToken);
                }
#pragma warning disable CC0004 // Catch block cannot be empty
                catch
                {
                    // ignored
                }
#pragma warning restore CC0004 // Catch block cannot be empty
            }
        }
    }
}
