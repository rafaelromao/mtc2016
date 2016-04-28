using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MTC2016
{
    public class Settings
    {
        public string ConfirmSubscription { get; set; }

        public string SubscriptionFailed { get; set; }

        public string ConnectionString { get; set; }

        public string Reminder { get; set; }

        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTimeOffset ReminderTime { get; set; }
    }
}
