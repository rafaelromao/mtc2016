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
        private readonly IArtificialInteligenceExtension _artificialInteligenceExtension;
        private readonly Settings _settings;
        private readonly string _imageAnswer;

        public ImageMessageReceiver(IMessagingHubSender sender, IArtificialInteligenceExtension artificialInteligenceExtension, Settings settings)
        {
            _sender = sender;
            _artificialInteligenceExtension = artificialInteligenceExtension;
            _settings = settings;
            try
            {
                _imageAnswer = _artificialInteligenceExtension.GetAnswerAsync(_settings.ImageConfirmation).Result;
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
}
