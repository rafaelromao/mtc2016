using System;
using System.Text.RegularExpressions;
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
    public class QuestionMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly IArtificialInteligenceExtension _artificialInteligenceExtension;
        private readonly Settings _settings;
        private readonly string _defaultAnswer;

        public QuestionMessageReceiver(IMessagingHubSender sender, IArtificialInteligenceExtension artificialInteligenceExtension, Settings settings)
        {
            _sender = sender;
            _artificialInteligenceExtension = artificialInteligenceExtension;
            _settings = settings;
            try
            {
                _defaultAnswer = _artificialInteligenceExtension.GetAnswerAsync(_settings.CouldNotUnderstand).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception when querying for {_settings.CouldNotUnderstand}: {e}");
                _defaultAnswer = _settings.GeneralError;
            }
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            try
            {
                var question = message.Content?.ToString() ?? string.Empty;
                var answer = !IsValidQuestion(question)
                    ? _defaultAnswer
                    : await _artificialInteligenceExtension.GetAnswerAsync(question);

                if (string.IsNullOrWhiteSpace(answer))
                {
                    answer = await _artificialInteligenceExtension.GetAnswerAsync(_settings.Quote);
                }

                if (string.IsNullOrWhiteSpace(answer))
                {
                    await _sender.SendMessageAsync(_defaultAnswer, message.From, cancellationToken);
                }
                else
                {
                    answer = await ExtractAndSendImagesAsync(answer, message.From, cancellationToken);

                    await _sender.SendMessageAsync(answer, message.From, cancellationToken);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception when querying for {message.Content}: {e}");
                try
                {
                    await _sender.SendMessageAsync(_defaultAnswer, message.From, cancellationToken);
                }
#pragma warning disable CC0004 // Catch block cannot be empty
                catch
                {
                    // ignored
                }
#pragma warning restore CC0004 // Catch block cannot be empty
            }
        }

        private async Task<string> ExtractAndSendImagesAsync(string answer, Node from, CancellationToken cancellationToken)
        {
            var match = Regex.Match(answer, "\\[(.*?)\\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (match.Success)
            {
                foreach (Capture capture in match.Captures)
                {
                    answer = answer.Replace(capture.Value, string.Empty).Trim();
                    await SendImagesAsync(capture.Value.Replace("[", "").Replace("]", ""), from, cancellationToken);
                }
            }
            return answer;
        }

        private async Task SendImagesAsync(string uri, Node from, CancellationToken cancellationToken)
        {
            var mediaLink = new MediaLink
            {
                Type = MediaType.Parse("image/jpg"),
                Uri = new Uri(uri)
            };
            await _sender.SendMessageAsync(mediaLink, from, cancellationToken);
        }

        private bool IsValidQuestion(string question)
        {
            // Does not allow questions starting with $ or %
            return !question.StartsWith("$") && 
                   !question.StartsWith(_settings.SchedulePrefix) && 
                   !question.StartsWith(_settings.FeedbackPrefix) &&
                   !question.StartsWith(_settings.RatingPrefix);
        }
    }
}
