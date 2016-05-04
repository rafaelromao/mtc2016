using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using Lime.Messaging.Contents;
using Lime.Protocol;
using MTC2016.Configuration;
using Takenet.MessagingHub.Client.Sender;

namespace MTC2016.Scheduler
{
    public sealed class SchedulerExtension : ISchedulerExtension, IDisposable
    {
        private readonly IJobScheduler _jobScheduler;
        private readonly IMessagingHubSender _sender;
        private readonly ObjectCache _cache;

        public SchedulerExtension(IServiceProvider serviceProvider, IJobScheduler jobScheduler, IMessagingHubSender sender, Settings settings)
        {
            _cache = new MemoryCache(nameof(MTC2016));
            _jobScheduler = jobScheduler;
            _sender = sender;

            _jobScheduler.Start();
        }

        public Task ScheduleAsync(Func<Task<IEnumerable<Identity>>> getRecipientsAsync, IEnumerable<ScheduledMessage> scheduledMessages, CancellationToken cancellationToken)
        {
            var scheduledMessagesGroup = scheduledMessages.GroupBy(a => a.Time);
            foreach (var scheduledMessageGroup in scheduledMessagesGroup)
            {
                _jobScheduler.Schedule(
                    () => SendScheduledMessages(scheduledMessageGroup, getRecipientsAsync, cancellationToken),
                    scheduledMessageGroup.Key);
            }
            return Task.CompletedTask;
        }

        // ReSharper disable once MemberCanBePrivate.Global - This method MUST be public to be seen by HangFire
        public void SendScheduledMessages(IEnumerable<ScheduledMessage> scheduledMessages, Func<Task<IEnumerable<Identity>>> getRecipientsAsync, CancellationToken cancellationToken)
        {
            SendScheduledMessagesAsync(scheduledMessages, getRecipientsAsync, cancellationToken).Wait(cancellationToken);
        }

        private async Task SendScheduledMessagesAsync(IEnumerable<ScheduledMessage> scheduledMessages, Func<Task<IEnumerable<Identity>>> getRecipientsAsync, CancellationToken cancellationToken)
        {
            var recipients = (getRecipientsAsync != null ? await getRecipientsAsync.Invoke() : null) ?? new Identity[0];

            scheduledMessages = scheduledMessages as ScheduledMessage[] ?? scheduledMessages.ToArray();

            foreach (var recipient in recipients)
            {
                foreach (var scheduledMessage in scheduledMessages)
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
                        await _sender.SendMessageAsync(message, cancellationToken);
                        _cache.Add(message.ToKey(), message, DateTimeOffset.Now.AddMinutes(1));
                    }
                }
            }
        }

        public void Dispose()
        {
            _jobScheduler.Stop();
        }
    }
}