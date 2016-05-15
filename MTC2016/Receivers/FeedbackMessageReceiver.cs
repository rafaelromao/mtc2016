using System;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using MTC2016.ArtificialInteligence;
using MTC2016.Configuration;
using Takenet.MessagingHub.Client;
using Takenet.MessagingHub.Client.Listener;
using Takenet.MessagingHub.Client.Sender;

namespace MTC2016.Receivers
{
    public class FeedbackMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly Settings _settings;
        private readonly string _feedbackAnswer;
        private readonly string _feedbackFailed;

        public FeedbackMessageReceiver(IMessagingHubSender sender, IApiAiForStaticContent apiAi, IFeedbackRepository feedbackRepository, Settings settings)
        {
            _sender = sender;
            _feedbackRepository = feedbackRepository;
            _settings = settings;
            try
            {
                _feedbackAnswer = apiAi.GetAnswerAsync(_settings.FeedbackConfirmation).Result;
                _feedbackFailed = apiAi.GetAnswerAsync(_settings.FeedbackFailed).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception when querying for {_settings.FeedbackConfirmation}: {e}");
                _feedbackAnswer = _settings.GeneralError;
            }
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            var text = message.Content?.ToString() ?? string.Empty;

            var feedback = new Feedback
            {
                From = message.From,
                When = DateTimeOffset.Now,
                Text = text,
                Type = FeedbackType.Comment
            };

            var ok = await _feedbackRepository.AddFeedbackAsync(feedback);
            if (ok)
            {
                await _sender.SendMessageAsync(_feedbackAnswer, message.From, cancellationToken);
            }
            else
            {
                await _sender.SendMessageAsync(_feedbackFailed, message.From, cancellationToken);
            }
        }
    }
}
