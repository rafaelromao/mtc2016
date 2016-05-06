using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lime.Protocol;

namespace MTC2016.Scheduler
{
    public class Schedule
    {
        public Func<Task<IEnumerable<Identity>>> Recipients { get; set; }
        public IEnumerable<ScheduledMessage> ScheduledMessages { get; set; }
    }
}