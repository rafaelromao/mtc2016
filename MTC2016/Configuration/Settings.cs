using MTC2016.Scheduler;

namespace MTC2016.Configuration
{
    public class Settings
    {
        public string ConnectionString { get; set; }

        public string TesterIdentifier { get; set; }
        public string TesterAccessKey { get; set; }

        public Messages Messages { get; set; }
        public ScheduledMessage[] ScheduledMessages { get; set; }
    }

    public class Messages
    {
        public string ConfirmSubscription { get; set; }

        public string SubscriptionFailed { get; set; }

        public string NotSubscribed { get; set; }

        public string ConfirmSubscriptionCancellation { get; set; }
    }
}
