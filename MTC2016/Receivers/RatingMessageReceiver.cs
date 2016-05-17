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
        private readonly string _ratingConfirmation;
        private readonly string _ratingFailed;

        public RatingMessageReceiver(IMessagingHubSender sender, IApiAiForStaticContent apiAi, IFeedbackRepository feedbackRepository, Settings settings)
        {
            _sender = sender;
            _feedbackRepository = feedbackRepository;
            try
            {
                _ratingConfirmation = apiAi.GetAnswerAsync(settings.RatingConfirmation).Result;
                _ratingFailed = apiAi.GetAnswerAsync(settings.RatingFailed).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception when querying for {settings.FeedbackConfirmation}: {e}");
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

            try
            {
                await _feedbackRepository.AddAsync(feedback);

                await _sender.SendMessageAsync(_ratingConfirmation, message.From, cancellationToken);
            }
            catch
            {
                await _sender.SendMessageAsync(_ratingFailed, message.From, cancellationToken);
            }
        }
    }
}
