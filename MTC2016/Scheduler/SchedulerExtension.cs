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
        private string _badRating;
        private string _regularRating;
        private string _goodRating;

        public SchedulerExtension(
            IApiAiForStaticContent apiAi, IJobScheduler jobScheduler,
            IDistributionListExtension distributionListExtension, Settings settings)
        {
            _apiAi = apiAi;
            _jobScheduler = jobScheduler;
            _distributionListExtension = distributionListExtension;
            _settings = settings;
        }

        private async Task SortRatingOptionsAsync()
        {
            try
            {
                _badRating = await _apiAi.GetAnswerAsync(_settings.BadRating);
                _regularRating = await _apiAi.GetAnswerAsync(_settings.RegularRating);
                _goodRating = await _apiAi.GetAnswerAsync(_settings.GoodRating);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception when querying for {_settings.CouldNotUnderstand}: {e}");
                _badRating = "Ruim";
                _regularRating = "Regular";
                _goodRating = "Bom";
            }
        }

        protected virtual async Task ScheduleAsync(IEnumerable<ScheduledMessage> messagesToBeScheduled)
        {
            var recipients = (await _distributionListExtension.GetRecipientsAsync(CancellationToken.None)).ToArray();
            foreach (var messageToBeScheduled in messagesToBeScheduled.AsParallel())
            {
                foreach (var recipient in recipients.AsParallel())
                {
                    var message = new Message
                    {
                        Id = Guid.NewGuid().ToString(),
                        Content = GetMessageToBeScheduledAccordingToDestinationDomain(messageToBeScheduled, recipient),
                        To = recipient.ToNode()
                    };

                    await _jobScheduler.ScheduleAsync(message, messageToBeScheduled.Time);
                }
            }
        }

        private Document GetMessageToBeScheduledAccordingToDestinationDomain(ScheduledMessage messageToBeScheduled, Identity recipient)
        {
            var domain = GetDomain(recipient);
            switch (domain)
            {
                case Domains.Omni:
                    return messageToBeScheduled.OmniMessage;
                case Domains.Tangram:
                    return messageToBeScheduled.TangramMessage;
                default:
                    return messageToBeScheduled.DefaultMessage;
            }
        }

        protected virtual string GetDomain(Identity domain)
        {
            return domain.Domain;
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
            foreach (var schedule in schedules.AsParallel())
            {
                var text = (await _apiAi.GetIntentAsync(schedule.Id)).Responses.First().Speech.FirstOrDefault();

                await SortRatingOptionsAsync();

                var defaultRatingOptions = ExtractRatingFromText(text);
                var tangramRatingOptions = ExtractRatingFromText(text, Domains.Tangram);
                var omniRatingOptions = ExtractRatingFromText(text, Domains.Omni);
                if (defaultRatingOptions != null)
                {
                    // Rating request
                    result.Add(new ScheduledMessage
                    {
                        Time = DateTimeOffset.Parse(schedule.Name.Substring(schedulePrefix.Length)),
                        DefaultMessage = defaultRatingOptions,
                        TangramMessage = tangramRatingOptions,
                        OmniMessage = omniRatingOptions
                    });
                }
                else
                {
                    // Text message
                    result.Add(new ScheduledMessage
                    {
                        Time = DateTimeOffset.Parse(schedule.Name.Substring(schedulePrefix.Length)),
                        DefaultMessage = new PlainText { Text = text },
                        TangramMessage = new PlainText { Text = text },
                        OmniMessage = new PlainText { Text = text }
                    });
                }
            }
            return result;
        }

        public async Task UpdateSchedulesAsync()
        {
            await ScheduleAsync(await GetMessagesToBeScheduled());
        }

        private Document ExtractRatingFromText(string text, string domain = null)
        {
            var match = Regex.Match(text, "\\[(.*?)\\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var capture = match.Captures[0];
                text = text.Replace(capture.Value, string.Empty).Trim();
                int order;
                switch (domain)
                {
                    case Domains.Omni:
                    case Domains.Tangram:
                        order = int.Parse(capture.Value.Replace("[", "").Replace("]", ""));
                        return CreateOmniAndTangramRatingOptions(text, order);
                    default:
                        order = int.Parse(capture.Value.Replace("[", "").Replace("]", ""));
                        return CreateDefaultRatingOptions(text, order);
                }
            }
            return null;
        }

        private Select CreateDefaultRatingOptions(string text, int order)
        {
            var select = new Select
            {
                Text = text,
                Options = new[]
                {
                    CreateRatingOption(null, _badRating, int.Parse($"{order}1")),
                    CreateRatingOption(null, _regularRating, int.Parse($"{order}2")),
                    CreateRatingOption(null, _goodRating, int.Parse($"{order}3")),
                }
            };
            return select;
        }

        private static SelectOption CreateRatingOption(int? order, string rating, object value = null)
        {
            value = value ?? order;
            return new SelectOption
            {
                Order = order,
                Text = rating,
                Value = new PlainText { Text = $"{value}" }
            };
        }

        private Document CreateOmniAndTangramRatingOptions(string text, int order)
        {
            var bad = $"{int.Parse($"{order}1")} {_badRating}";
            var regular = $"{int.Parse($"{order}2")} {_regularRating}";
            var good = $"{int.Parse($"{order}3")} {_goodRating}";
            var select = new PlainText
            {
                Text = $"{text} Escolha:{bad};{regular};{good}"
            };
            return select;
        }
    }
}