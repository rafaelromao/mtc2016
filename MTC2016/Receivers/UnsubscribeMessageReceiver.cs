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
    public class UnsubscribeMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly IApiAI _apiAi;
        private readonly IDistributionListExtension _distributionListExtension;
        private readonly Settings _settings;
        private string _defaultAnswer;
        private string _confirmUnsubscription;
        private string _notSubscribed;
        private string _unsubscriptionFailed;

        public UnsubscribeMessageReceiver(IMessagingHubSender sender, IApiAI apiAi,
            IDistributionListExtension distributionListExtension, Settings settings)
        {
            _sender = sender;
            _apiAi = apiAi;
            _distributionListExtension = distributionListExtension;
            _settings = settings;
            try
            {
                InitializeDefaultAnswersAsync().Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception when querying for {_settings.CouldNotUnderstand}: {e}");
                _defaultAnswer = _settings.GeneralError;
            }
        }

        private async Task InitializeDefaultAnswersAsync()
        {
            _defaultAnswer = await _apiAi.GetAnswerAsync(_settings.CouldNotUnderstand);
            _confirmUnsubscription = await _apiAi.GetAnswerAsync(_settings.ConfirmSubscriptionCancellation);
            _notSubscribed = await _apiAi.GetAnswerAsync(_settings.NotSubscribed);
            _unsubscriptionFailed = await _apiAi.GetAnswerAsync(_settings.UnsubscriptionFailed);
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            try
            {
                if (!await _distributionListExtension.ContainsAsync(message.From, cancellationToken))
                {
                    await _sender.SendMessageAsync(_notSubscribed, message.From, cancellationToken);
                }
                else if (await _distributionListExtension.RemoveAsync(message.From, cancellationToken))
                {
                    await _sender.SendMessageAsync(_confirmUnsubscription, message.From, cancellationToken);
                }
                else
                {
                    await _sender.SendMessageAsync(_unsubscriptionFailed, message.From, cancellationToken);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception when trying to unsubscribe: {e}");
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
