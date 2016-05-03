using System.Linq;
using System.Text.RegularExpressions;
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
    public class QuestionMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly IArtificialInteligenceExtension _artificialInteligenceExtension;
        private readonly Settings _settings;
        private readonly string _defaulAnswer;

        public QuestionMessageReceiver(IMessagingHubSender sender, IArtificialInteligenceExtension artificialInteligenceExtension, Settings settings)
        {
            _sender = sender;
            _artificialInteligenceExtension = artificialInteligenceExtension;
            _settings = settings;
            try
            {
                _defaulAnswer = _artificialInteligenceExtension.GetAnswerAsync(_settings.CouldNotUnderstand).Result;
            }
            catch
            {
                _defaulAnswer = _settings.GeneralError;
            }
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            var question = message.Content?.ToString() ?? string.Empty;
            var answer = !IsValidQuestion(question) ? _defaulAnswer : await _artificialInteligenceExtension.GetAnswerAsync(question);
            if (answer == null)
            {
                await _sender.SendMessageAsync(_defaulAnswer, message.From, cancellationToken);
            }
            else
            {
                await _sender.SendMessageAsync(answer, message.From, cancellationToken);
            }
        }

        private bool IsValidQuestion(string question)
        {
            // Does not allow questions starting with $ or %
            return !question.StartsWith("$") && !question.StartsWith(_settings.SchedulePrefix);
        }
    }
}
