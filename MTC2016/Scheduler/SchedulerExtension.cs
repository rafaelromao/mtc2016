using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Lime.Messaging.Contents;
using Lime.Protocol;
using MTC2016.ArtificialInteligence;
using MTC2016.Configuration;
using MTC2016.DistributionList;

namespace MTC2016.Scheduler
{
    public class SchedulerExtension : ISchedulerExtension
    {
        private readonly IApiAiForStaticContent _apiAi;
        private readonly IJobScheduler _jobScheduler;
        private readonly IDistributionListExtension _distributionListExtension;
        private readonly Settings _settings;

        public SchedulerExtension(
            IApiAiForStaticContent apiAi, IJobScheduler jobScheduler,
            IDistributionListExtension distributionListExtension, Settings settings)
        {
            _apiAi = apiAi;
            _jobScheduler = jobScheduler;
            _distributionListExtension = distributionListExtension;
            _settings = settings;
        }

        protected virtual async Task ScheduleAsync(IEnumerable<ScheduledMessage> messagesToBeScheduled)
        {
            var recipients = (await _distributionListExtension.GetRecipientsAsync(CancellationToken.None)).ToArray();
            foreach (var messageToBeScheduled in messagesToBeScheduled)
            {
                foreach (var recipient in recipients)
                {
                    var message = new Message
                    {
                        Id = Guid.NewGuid().ToString(),
                        Content = messageToBeScheduled.Message,
                        To = recipient.ToNode()
                    };

                    await _jobScheduler.ScheduleAsync(message, messageToBeScheduled.Time);
                }
            }
        }

        protected virtual async Task<IEnumerable<ScheduledMessage>> GetMessagesToBeScheduled()
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

        public async Task UpdateSchedulesAsync()
        {
            await ScheduleAsync(await GetMessagesToBeScheduled());
        }

        private Select ExtractRatingFromText(string text)
        {
            var match = Regex.Match(text, "\\[(.*?)\\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var capture = match.Captures[0];
                text = text.Replace(capture.Value, string.Empty).Trim();
                var order = int.Parse(capture.Value.Replace("[", "").Replace("]", ""));
                return CreateRatingOptions(text, order);
            }
            return null;
        }

        private Select CreateRatingOptions(string text, int order)
        {
            var select = new Select
            {
                Text = text,
                Options = new[]
                {
                    CreateRatingOption(int.Parse($"{order}1"), _settings.BadRating),
                    CreateRatingOption(int.Parse($"{order}2"), _settings.RegularRating),
                    CreateRatingOption(int.Parse($"{order}3"), _settings.GoodRating),
                }
            };
            return select;
        }

        private SelectOption CreateRatingOption(int order, string rating)
        {
            return new SelectOption
            {
                Order = order,
                Text = rating,
                Value = new PlainText { Text = $"{order}" }
            };
        }
    }
}