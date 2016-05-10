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
        private readonly IArtificialInteligenceExtension _artificialInteligenceExtension;
        private readonly Settings _settings;
        //private readonly string _ratingTitle;
        private readonly string _ratingText;
        private readonly string _ratingConfirmation;
        //private readonly string _ratingFailed;
        //private readonly ObjectCache _session = new MemoryCache(nameof(MTC2016));

        public RatingMessageReceiver(IMessagingHubSender sender, IArtificialInteligenceExtension artificialInteligenceExtension, Settings settings)
        {
            _sender = sender;
            _artificialInteligenceExtension = artificialInteligenceExtension;
            _settings = settings;
            //_ratingTitle = _artificialInteligenceExtension.GetAnswerAsync(_settings.RatingTitle).Result;
            _ratingText = _artificialInteligenceExtension.GetAnswerAsync(_settings.RatingText).Result;
            _ratingConfirmation = _artificialInteligenceExtension.GetAnswerAsync(_settings.RatingConfirmation).Result;
            //_ratingFailed = _artificialInteligenceExtension.GetAnswerAsync(_settings.RatingFailed).Result;
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            var text = message.Content?.ToString().ToLower() ?? string.Empty;
            //var from = message.From.ToNode().ToString();
            //var feedbackId = CreateRatingId(_settings, from, DateTime.Now);

            switch (text)
            {
                case "avaliação":
                case "avaliacao":
                case "avaliar":
                    var ratingOptions = new Select
                    {
                        Text = _ratingText,
                        Options = new[]
                        {
                            new SelectOption { Order = 1, Text = "Péssimo" },
                            new SelectOption { Order = 2, Text = "Ruim" },
                            new SelectOption { Order = 3, Text = "Razoável" },
                            new SelectOption { Order = 4, Text = "Bom" },
                            new SelectOption { Order = 5, Text = "Excelente" }
                        }
                    };
                    await _sender.SendMessageAsync(ratingOptions, message.From, cancellationToken);
                    break;
                case "Péssimo":
                case "Ruim":
                case "Razoável":
                case "Bom":
                case "Excelente":
                    //var ok = await _artificialInteligenceExtension.AddFeedbackAsync(feedbackId, feedback);
                    //if (ok)
                    //{
                        await _sender.SendMessageAsync(_ratingConfirmation, message.From, cancellationToken);
                    //}
                    //else
                    //{
                    //    await _sender.SendMessageAsync(_settings.FeedbackFailed, message.From, cancellationToken);
                    //}
                    break;
            }
        }

        //public static string CreateRatingId(Settings settings, string from, DateTime time)
        //{
        //    return $"{settings.RatingPrefix} from_{from}_at_{time.ToString("yyyyMMddhhnn")}";
        //}
    }
}
