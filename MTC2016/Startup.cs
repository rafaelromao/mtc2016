using System;
using System.Threading;
using System.Threading.Tasks;
using Takenet.MessagingHub.Client.Listener;

namespace MTC2016
{
    public class Startup : IStartable
    {
        private readonly Settings _settings;
        private readonly IDistributionListExtension _distributionListExtension;
        private readonly ISchedulerExtension _schedulerExtension;

        public Startup(Settings settings, IDistributionListExtension distributionListExtension, ISchedulerExtension schedulerExtension)
        {
            _settings = settings;
            _distributionListExtension = distributionListExtension;
            _schedulerExtension = schedulerExtension;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await ScheduleEventReminderAsync();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
            }
        }

        private async Task ScheduleEventReminderAsync()
        {
            var reminderTime = _settings.ReminderTime;
            var recipients = await _distributionListExtension.GetAllAsync();
            await _schedulerExtension.ScheduleAsync(_settings.Reminder, recipients, reminderTime);
        }
    }
}
