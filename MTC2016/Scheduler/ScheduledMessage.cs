using System;
using Lime.Protocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MTC2016.Scheduler
{
    public class ScheduledMessage
    {
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTimeOffset Time { get; set; }
        public Document DefaultMessage { get; set; }
        public Document TangramMessage { get; set; }
        public Document OmniMessage { get; set; }
    }
}