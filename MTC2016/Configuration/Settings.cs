namespace MTC2016.Configuration
{
    public class Settings
    {
        public string ConnectionString { get; set; }
        public string ApiaiUri { get; set; }
        public string ApiaiClientToken { get; set; }

        public string SchedulePrefix { get; set; }
        public string GeneralError { get; set; }
        public string CouldNotUnderstand { get; set; }
        public string SubscriptionFailed { get; set; }
        public string ConfirmSubscription { get; set; }
        public string AlreadySubscribed { get; set; }
        public string NotSubscribed { get; set; }
        public string ConfirmSubscriptionCancellation { get; set; }
        public string UnsubscriptionFailed { get; set; }
    }
}
