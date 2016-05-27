using System;
using System.Collections.Generic;
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
        private readonly IApiAiForDynamicContent _apiAi;
        private readonly Settings _settings;

        public QuestionMessageReceiver(IMessagingHubSender sender, IApiAiForDynamicContent apiAi, Settings settings)
        {
            _sender = sender;
            _apiAi = apiAi;
            _settings = settings;
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            try
            {
                var question = message.Content?.ToString() ?? string.Empty;
                var answer = !IsValidQuestion(question)
                    ? await GetDefaultAnswerAsync()
                    : await _apiAi.GetAnswerAsync(question);

                if (string.IsNullOrWhiteSpace(answer))
                {
                    answer = await GetDefaultAnswerAsync();
                }

                if (string.IsNullOrWhiteSpace(answer))
                {
                    await _sender.SendMessageAsync(await GetDefaultAnswerAsync(), message.From, cancellationToken);
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
                    await _sender.SendMessageAsync(await GetDefaultAnswerAsync(), message.From, cancellationToken);
                }
#pragma warning disable CC0004 // Catch block cannot be empty
                catch
                {
                    // ignored
                }
#pragma warning restore CC0004 // Catch block cannot be empty
            }
        }

        private async Task<string> GetDefaultAnswerAsync()
        {
            try
            {
                return await _apiAi.GetAnswerAsync(_settings.CouldNotUnderstand);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception when querying for {_settings.CouldNotUnderstand}: {e}");
                return _settings.GeneralError;
            }
        }

        protected virtual async Task<string> ExtractAndSendImagesAsync(string answer, Node from, CancellationToken cancellationToken)
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
            var domain = GetDomain(@from);
            Document document;
            switch (domain)
            {
                case Domains.Omni:
                    var jsondocument = new JsonDocument(new MediaType("application", "vnd.omni.text", "json"));
                    var attachments = new List<IDictionary<string, object>>();
                    var attachment = new Dictionary<string, object>
                    {
                        {"mimeType", "image/jpeg"},
                        {"mediaType", "image"},
                        {"size", 100},
                        {"remoteUri", uri},
                        {"thumbnailUri", uri}
                    };
                    attachments.Add(attachment);
                    jsondocument.Add(nameof(attachments), attachments);
                    document = jsondocument;
                    break;
                default:
                    document = new MediaLink
                    {
                        Type = MediaType.Parse("image/jpg"),
                        Uri = new Uri(uri)
                    };
                    break;
            }

            await _sender.SendMessageAsync(document, from, cancellationToken);
        }

        protected virtual string GetDomain(Identity from)
        {
            return from.Domain;
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
