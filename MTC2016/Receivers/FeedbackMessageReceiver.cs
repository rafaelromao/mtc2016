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
        private readonly IApiAI _apiAi;
        private readonly Settings _settings;
        private readonly string _feedbackAnswer;

        public FeedbackMessageReceiver(IMessagingHubSender sender, IApiAI apiAi, Settings settings)
        {
            _sender = sender;
            _apiAi = apiAi;
            _settings = settings;
            try
            {
                _feedbackAnswer = _apiAi.GetAnswerAsync(_settings.FeedbackConfirmation).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception when querying for {_settings.FeedbackConfirmation}: {e}");
                _feedbackAnswer = _settings.GeneralError;
            }
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            var feedback = message.Content?.ToString() ?? string.Empty;

            var from = message.From.ToNode().ToString();
            var feedbackId = CreateFeedbackId(_settings, from, DateTime.Now);

            var ok = await _apiAi.AddFeedbackAsync(feedbackId, feedback);
            if (ok)
            {
                await _sender.SendMessageAsync(_feedbackAnswer, message.From, cancellationToken);
            }
            else
            {
                await _sender.SendMessageAsync(_settings.FeedbackFailed, message.From, cancellationToken);
            }
        }

        public static string CreateFeedbackId(Settings settings, string from, DateTime time)
        {
            from = settings.EncodeIdentity(from);
            return $"{settings.FeedbackPrefix}from_{from}_at_{time.ToString("yyyyMMddhhmm")}";
        }
    }
}
