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
    public class ImageMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly string _imageAnswer;

        public ImageMessageReceiver(IMessagingHubSender sender, IApiAiForStaticContent apiAi, Settings settings)
        {
            _sender = sender;
            try
            {
                _imageAnswer = apiAi.GetAnswerAsync(settings.ImageConfirmation).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception when querying for {settings.FeedbackConfirmation}: {e}");
                _imageAnswer = settings.GeneralError;
            }
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
             await _sender.SendMessageAsync(_imageAnswer, message.From, cancellationToken);
        }
    }

    public class ImageJpegMessageReceiver : ImageMessageReceiver
    {
        public ImageJpegMessageReceiver(IMessagingHubSender sender, IApiAiForStaticContent apiAi, Settings settings) : base(sender, apiAi, settings)
        {
        }
    }

    public class ImagePngMessageReceiver : ImageMessageReceiver
    {
        public ImagePngMessageReceiver(IMessagingHubSender sender, IApiAiForStaticContent apiAi, Settings settings) : base(sender, apiAi, settings)
        {
        }
    }
}
