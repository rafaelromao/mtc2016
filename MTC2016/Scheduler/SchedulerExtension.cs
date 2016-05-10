using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using Lime.Messaging.Contents;
using Lime.Protocol;
using MTC2016.ArtificialInteligence;
using MTC2016.Configuration;
using Takenet.MessagingHub.Client.Sender;

namespace MTC2016.Scheduler
{
    public class SchedulerExtension : ISchedulerExtension, IDisposable
    {
        private readonly IArtificialInteligenceExtension _artificialInteligenceExtension;
        private readonly IJobScheduler _jobScheduler;
        private readonly IMessagingHubSender _sender;
        private readonly Settings _settings;
        private readonly ObjectCache _cache;

        public SchedulerExtension(IArtificialInteligenceExtension artificialInteligenceExtension, IJobScheduler jobScheduler, IMessagingHubSender sender, Settings settings)
        {
            _cache = new MemoryCache(nameof(MTC2016));
            _artificialInteligenceExtension = artificialInteligenceExtension;
            _jobScheduler = jobScheduler;
            _sender = sender;
            _settings = settings;
        }

        public virtual async Task ScheduleAsync(
            Func<Task<IEnumerable<Identity>>> recipients,
            IEnumerable<ScheduledMessage> scheduledMessages,
            CancellationToken cancellationToken)
        {
            foreach (var group in scheduledMessages.GroupBy(s => s.Time))
            {
                var schedule = new Schedule
                {
                    Recipients = recipients,
                    ScheduledMessages = group
                };

                await _jobScheduler.ScheduleAsync(async () => await SendScheduledMessagesAsync(schedule), group.Key, cancellationToken);
            }
        }

        private async Task SendScheduledMessagesAsync(Schedule schedule)
        {
            foreach (var recipient in await schedule.Recipients())
            {
                foreach (var scheduledMessage in schedule.ScheduledMessages)
                {
                    var message = new Message
                    {
                        Id = Guid.NewGuid(),
                        Content = new PlainText
                        {
                            Text = scheduledMessage.Text
                        },
                        To = recipient.ToNode()
                    };

                    // Avoid sending the same message twice within the same minute
                    if (!_cache.Contains(message.ToKey()))
                    {
                        Console.WriteLine($"{message} -> {recipient}");
                        await _sender.SendMessageAsync(message);
                        _cache.Add(message.ToKey(), message, DateTimeOffset.Now.AddMinutes(1));
                    }
                }
            }
        }

        public virtual async Task<IEnumerable<ScheduledMessage>> GetScheduledMessagesAsync(CancellationToken cancellationToken)
        {
            var result = new List<ScheduledMessage>();
            var intents = await _artificialInteligenceExtension.GetIntentsAsync();
            var schedulePrefix = _settings.SchedulePrefix;
            var schedules = intents.Where(i => i.Name.StartsWith(schedulePrefix));
            // Ignore expired schedules
            schedules =
                schedules.Where(s => DateTimeOffset.Parse(s.Name.Substring(schedulePrefix.Length)) >= DateTime.Now);
            foreach (var schedule in schedules)
            {
                result.Add(new ScheduledMessage
                {
                    Time = DateTimeOffset.Parse(schedule.Name.Substring(schedulePrefix.Length)),
                    Text = (await _artificialInteligenceExtension.GetIntentAsync(schedule.Id)).Responses.First().Speech
                });
            }
            return result;
        }

        public void Dispose()
        {
            _jobScheduler.Dispose();
        }
    }
}