using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lime.Protocol;

namespace MTC2016
{
    public interface ISchedulerExtension
    {
        Task ScheduleAsync(string reminder, IEnumerable<Identity> recipients, DateTimeOffset reminderTime);
    }
}