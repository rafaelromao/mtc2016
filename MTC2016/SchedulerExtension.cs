using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Lime.Messaging.Contents;
using Lime.Protocol;
using Takenet.MessagingHub.Client;
using Takenet.MessagingHub.Client.Messages;
using Takenet.MessagingHub.Client.Sender;

namespace MTC2016
{
    public sealed class SchedulerExtension : ISchedulerExtension, IDisposable
    {
        private readonly IMessagingHubSender _sender;
        private readonly BackgroundJobServer _server;
        private readonly ObjectCache _cache;

        public SchedulerExtension(IServiceProvider serviceProvider, IMessagingHubSender sender, Settings settings)
        {
            _cache = new MemoryCache(nameof(MTC2016));
            _sender = sender;

            GlobalConfiguration.Configuration
                .UseActivator(new ContainerJobActivator(serviceProvider))
                .UseSqlServerStorage(settings.ConnectionString);

            _server = new BackgroundJobServer();
        }

        public Task ScheduleAsync(DateTimeOffset reminderTime, string message, CancellationToken cancellationToken, params Identity[] recipients)
        {
            BackgroundJob.Schedule(() => SendMessage(message, cancellationToken, recipients), reminderTime);
            return Task.CompletedTask;
        }

        // ReSharper disable once MemberCanBePrivate.Global - This method MUST be public to be seen by HangFire
        public void SendMessage(string message, CancellationToken cancellationToken, IEnumerable<Identity> recipients)
        {
            SendMessageAsync(message, cancellationToken, recipients).Wait(cancellationToken);
        }

        private async Task SendMessageAsync(string text, CancellationToken cancellationToken, IEnumerable<Identity> recipients)
        {
            foreach (var recipient in recipients)
            {
                var message = new Message
                {
                    Id = Guid.NewGuid(),
                    Content = new PlainText
                    {
                        Text = text
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

        public void Dispose()
        {
            _server.Dispose();
        }
    }
}