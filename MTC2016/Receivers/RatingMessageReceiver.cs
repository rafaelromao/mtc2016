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
    public class RatingMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly Settings _settings;
        private readonly string _ratingConfirmation;
        private readonly string _ratingFailed;

        public RatingMessageReceiver(IMessagingHubSender sender, IApiAiForStaticContent apiAi, IFeedbackRepository feedbackRepository, Settings settings)
        {
            _sender = sender;
            _feedbackRepository = feedbackRepository;
            _settings = settings;
            try
            {
                _ratingConfirmation = apiAi.GetAnswerAsync(_settings.RatingConfirmation).Result;
                _ratingFailed = apiAi.GetAnswerAsync(_settings.RatingFailed).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception when querying for {_settings.FeedbackConfirmation}: {e}");
            }
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            var text = message.Content?.ToString().ToLower() ?? string.Empty;
            var feedback = new Feedback
            {
                From = message.From,
                When = DateTimeOffset.Now,
                Text = text,
                Type = FeedbackType.Rating
            };

            var ok = await _feedbackRepository.AddFeedbackAsync(feedback);
            if (ok)
            {
                await _sender.SendMessageAsync(_ratingConfirmation, message.From, cancellationToken);
            }
            else
            {
                await _sender.SendMessageAsync(_ratingFailed, message.From, cancellationToken);
            }
        }
    }
}
