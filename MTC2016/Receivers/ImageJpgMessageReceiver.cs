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
    public class ImageJpgMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly IApiAI _apiAi;
        private readonly Settings _settings;
        private readonly string _imageAnswer;

        public ImageJpgMessageReceiver(IMessagingHubSender sender, IApiAI apiAi, Settings settings)
        {
            _sender = sender;
            _apiAi = apiAi;
            _settings = settings;
            try
            {
                _imageAnswer = _apiAi.GetAnswerAsync(_settings.ImageConfirmation).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception when querying for {_settings.FeedbackConfirmation}: {e}");
                _imageAnswer = _settings.GeneralError;
            }
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
             await _sender.SendMessageAsync(_imageAnswer, message.From, cancellationToken);
        }
    }

    internal class ImageJpgJpegMessageReceiver : ImageJpgMessageReceiver
    {
        public ImageJpgJpegMessageReceiver(IMessagingHubSender sender, IApiAI apiAi, Settings settings) : base(sender, apiAi, settings)
        {
        }
    }

    internal class ImageJpgPngMessageReceiver : ImageJpgMessageReceiver
    {
        public ImageJpgPngMessageReceiver(IMessagingHubSender sender, IApiAI apiAi, Settings settings) : base(sender, apiAi, settings)
        {
        }
    }
}
