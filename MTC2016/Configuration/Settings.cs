using MTC2016.Scheduler;

namespace MTC2016.Configuration
{
    public class Settings
    {
        public string ConnectionString { get; set; }

        public Messages Messages { get; set; }

        public ScheduledMessage[] ScheduledMessages { get; set; }

        public Question[] Questions { get; set; }
    }
}
