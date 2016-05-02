using Lime.Protocol;

namespace MTC2016.Configuration
{
    public class Messages
    {
        public string ConfirmSubscription { get; set; }

        public string SubscriptionFailed { get; set; }

        public string NotSubscribed { get; set; }

        public string ConfirmSubscriptionCancellation { get; set; }

        public string AlreadySubscribed { get; set; }

        public string CouldNotUnderstandQuestion { get; set; }
    }
}