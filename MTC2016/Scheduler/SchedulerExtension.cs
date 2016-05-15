using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Lime.Messaging.Contents;
using Lime.Protocol;
using MTC2016.ArtificialInteligence;
using MTC2016.Configuration;
using MTC2016.DistributionList;
using Takenet.MessagingHub.Client.Sender;

namespace MTC2016.Scheduler
{
    public class SchedulerExtension : ISchedulerExtension, IDisposable
    {
        private readonly IApiAiForStaticContent _apiAi;
        private readonly IJobScheduler _jobScheduler;
        private readonly IMessagingHubSender _sender;
        private readonly Settings _settings;
        private readonly ObjectCache _cache;

        public SchedulerExtension(IApiAiForStaticContent apiAi, IJobScheduler jobScheduler, IMessagingHubSender sender, Settings settings)
        {
            _cache = new MemoryCache(nameof(MTC2016));
            _apiAi = apiAi;
            _jobScheduler = jobScheduler;
            _sender = sender;
            _settings = settings;
        }

        protected virtual async Task ScheduleAsync(IEnumerable<ScheduledMessage> messagesToBeScheduled, CancellationToken cancellationToken)
        {
            foreach (var messageToBeScheduled in messagesToBeScheduled)
            {
                var message = new Message
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = messageToBeScheduled.Message,
                    To = RecipientsRepository.Mtc2016.ToNode()
                };

                await _jobScheduler.ScheduleAsync(message, messageToBeScheduled.Time, cancellationToken);
            }
        }

        protected virtual async Task<IEnumerable<ScheduledMessage>> GetMessagesToBeScheduled(CancellationToken cancellationToken)
        {
            var result = new List<ScheduledMessage>();
            var intents = await _apiAi.GetIntentsAsync();
            var schedulePrefix = _settings.SchedulePrefix;
            var schedules = intents.Where(i => i.Name.StartsWith(schedulePrefix));
            // Ignore expired schedules
            schedules =
                schedules.Where(s => DateTimeOffset.Parse(s.Name.Substring(schedulePrefix.Length)) >= DateTime.Now);
            foreach (var schedule in schedules)
            {
                var text = (await _apiAi.GetIntentAsync(schedule.Id)).Responses.First().Speech;
                var ratingOptions = ExtractRatingFromText(text);
                if (ratingOptions != null)
                {
                    // Rating request
                    result.Add(new ScheduledMessage
                    {
                        Time = DateTimeOffset.Parse(schedule.Name.Substring(schedulePrefix.Length)),
                        Message = ratingOptions
                    });
                }
                else
                {
                    // Text message
                    result.Add(new ScheduledMessage
                    {
                        Time = DateTimeOffset.Parse(schedule.Name.Substring(schedulePrefix.Length)),
                        Message = new PlainText { Text = text }
                    });
                }
            }
            return result;
        }

        public async Task UpdateSchedulesAsync(CancellationToken cancellationToken)
        {
            await ScheduleAsync(await GetMessagesToBeScheduled(cancellationToken), cancellationToken);
        }

        private Select ExtractRatingFromText(string text)
        {
            var match = Regex.Match(text, "\\[(.*?)\\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var capture = match.Captures[0];
                text = text.Replace(capture.Value, string.Empty).Trim();
                return CreateRatingOptions(text, capture.Value.Replace("[", "").Replace("]", ""));
            }
            return null;
        }

        private Select CreateRatingOptions(string text, string toBeRated)
        {
            var select = new Select
            {
                Text = text,
                Options = new[]
                {
                    CreateRatingOption(toBeRated, _settings.BadRating),
                    CreateRatingOption(toBeRated, _settings.RegularRating),
                    CreateRatingOption(toBeRated, _settings.GoodRating),
                }
            };
            return select;
        }

        private SelectOption CreateRatingOption(string toBeRated, string rating)
        {
            return new SelectOption
            {
                Text = rating,
                Value = new PlainText { Text = $"{_settings.RatingPrefix}{toBeRated}:{rating}" }
            };
        }

        public void Dispose()
        {
            _jobScheduler.Dispose();
        }
    }
}