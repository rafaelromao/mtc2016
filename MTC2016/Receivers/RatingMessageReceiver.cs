using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lime.Messaging.Contents;
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
        private readonly IApiAI _apiAi;
        private readonly Settings _settings;
        private readonly string _ratingConfirmation;
        private readonly string _ratingFailed;

        public RatingMessageReceiver(IMessagingHubSender sender, IApiAI apiAi, Settings settings)
        {
            _sender = sender;
            _apiAi = apiAi;
            _settings = settings;
            _ratingConfirmation = _apiAi.GetAnswerAsync(_settings.RatingConfirmation).Result;
            _ratingFailed = _apiAi.GetAnswerAsync(_settings.RatingFailed).Result;
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            var ratting = message.Content?.ToString().ToLower() ?? string.Empty;
            var from = message.From.ToNode().ToString();
            var ratingId = CreateRatingId(_settings, from, DateTime.Now);

            var ok = await _apiAi.AddFeedbackAsync(ratingId, ratting);
            if (ok)
            {
                await _sender.SendMessageAsync(_ratingConfirmation, message.From, cancellationToken);
            }
            else
            {
                await _sender.SendMessageAsync(_settings.FeedbackFailed, message.From, cancellationToken);
            }
        }

        public static string CreateRatingId(Settings settings, string from, DateTime time)
        {
            from = settings.EncodeIdentity(from);
            return $"{settings.RatingPrefix}from_{from}_at_{time.ToString("yyyyMMddhhmm")}";
        }
    }
}
