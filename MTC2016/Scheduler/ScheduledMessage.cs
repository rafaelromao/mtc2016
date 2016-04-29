using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MTC2016.Scheduler
{
    public class ScheduledMessage
    {
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTimeOffset Time { get; set; }
        public string Text { get; set; }
    }
}