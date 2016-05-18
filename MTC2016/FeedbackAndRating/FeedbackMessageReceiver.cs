using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using MTC2016.ArtificialInteligence;
using MTC2016.Configuration;
using Takenet.MessagingHub.Client;
using Takenet.MessagingHub.Client.Listener;
using Takenet.MessagingHub.Client.Sender;

namespace MTC2016.FeedbackAndRating
{
    public class FeedbackMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly string _feedbackAnswer;
        private readonly string _feedbackFailed;

        public FeedbackMessageReceiver(IMessagingHubSender sender, IApiAiForStaticContent apiAi, IFeedbackRepository feedbackRepository, Settings settings)
        {
            _sender = sender;
            _feedbackRepository = feedbackRepository;
            try
            {
                _feedbackAnswer = apiAi.GetAnswerAsync(settings.FeedbackConfirmation).Result;
                _feedbackFailed = apiAi.GetAnswerAsync(settings.FeedbackFailed).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception when querying for {settings.FeedbackConfirmation}: {e}");
                _feedbackAnswer = settings.GeneralError;
            }
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {

            var text = message.Content?.ToString() ?? string.Empty;
            var parts = new List<string>();
            if (text.Length > FeedbackRepository.MaxTextSize)
            {
                while (text.Length > FeedbackRepository.MaxTextSize)
                {
                    parts.Add(text.Substring(0, Math.Min(FeedbackRepository.MaxTextSize, text.Length)));
                    text = text.Substring(FeedbackRepository.MaxTextSize);
                }
            }
            else
            {
                parts.Add(text);
            }

            try
            {
                foreach (var part in parts)
                {
                    var feedback = new Feedback
                    {
                        From = message.From,
                        When = DateTimeOffset.Now,
                        Text = part,
                        Type = FeedbackType.Comment
                    };

                    await _feedbackRepository.AddAsync(feedback);
                }
                await _sender.SendMessageAsync(_feedbackAnswer, message.From, cancellationToken);
            }
            catch (Exception ex)
            {
                await _sender.SendMessageAsync(_feedbackFailed, message.From, cancellationToken);
            }
        }
    }
}
