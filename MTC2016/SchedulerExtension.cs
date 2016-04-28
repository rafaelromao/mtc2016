using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Lime.Protocol;
using Takenet.MessagingHub.Client;
using Takenet.MessagingHub.Client.Sender;

namespace MTC2016
{
    public sealed class SchedulerExtension : ISchedulerExtension, IDisposable
    {
        private readonly IMessagingHubSender _sender;
        private readonly BackgroundJobServer _server;

        public SchedulerExtension(IServiceProvider serviceProvider, IMessagingHubSender sender, Settings settings)
        {
            _sender = sender;

            GlobalConfiguration.Configuration
                .UseActivator(new ContainerJobActivator(serviceProvider))
                .UseSqlServerStorage(settings.ConnectionString);

            _server = new BackgroundJobServer();
        }

        public Task ScheduleAsync(string message, IEnumerable<Identity> recipients, DateTimeOffset reminderTime)
        {
            BackgroundJob.Schedule(() => SendEventReminder(message, recipients.Select(i => i.ToString())), reminderTime);
            return Task.CompletedTask;
        }

        // ReSharper disable once MemberCanBePrivate.Global - This method MUST be public to be seen by HangFire
        public void SendEventReminder(string message, IEnumerable<string> recipients)
        {
            SendEventReminderAsync(message, recipients.Select(Identity.Parse)).Wait();
        }

        private async Task SendEventReminderAsync(string message, IEnumerable<Identity> recipients)
        {
            foreach (var recipient in recipients)
            {
                Console.WriteLine($"{message} -> {recipient}");
                await _sender.SendMessageAsync(message, recipient.ToNode());
            }
        }

        public void Dispose()
        {
            _server.Dispose();
        }
    }
}