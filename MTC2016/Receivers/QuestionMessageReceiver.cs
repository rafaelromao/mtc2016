using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using MTC2016.Configuration;
using Takenet.MessagingHub.Client;
using Takenet.MessagingHub.Client.Listener;
using Takenet.MessagingHub.Client.Sender;

namespace MTC2016.Receivers
{
    public class QuestionMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly Settings _settings;

        public QuestionMessageReceiver(IMessagingHubSender sender, Settings settings)
        {
            _sender = sender;
            _settings = settings;
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            var text = message.Content?.ToString() ?? string.Empty;
            var question = _settings.QuestionWithContent(text);
            if (question == null)
            {
                await
                    _sender.SendMessageAsync(_settings.Messages.CouldNotUnderstandQuestion, message.From,
                        cancellationToken);
            }
            else
            {
                await _sender.SendMessageAsync(question.Answer, message.From, cancellationToken);
            }
        }
    }
}
