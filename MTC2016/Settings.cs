using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MTC2016
{
    public class Settings
    {
        public string ConnectionString { get; set; }

        public Messages Messages { get; set; }
        public Reminder[] Reminders { get; set; }
    }

    public class Messages
    {
        public string ConfirmSubscription { get; set; }

        public string SubscriptionFailed { get; set; }

        public string NotSubscribed { get; set; }

        public string ConfirmSubscriptionCancellation { get; set; }
    }

    public class Reminder
    {
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTimeOffset Time { get; set; }
        public string Message { get; set; }
    }
}
