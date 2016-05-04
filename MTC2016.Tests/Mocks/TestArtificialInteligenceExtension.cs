using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lime.Protocol.Serialization;
using MTC2016.ArtificialInteligence;
using MTC2016.Configuration;
using MTC2016.Scheduler;

namespace MTC2016.Tests.Mocks
{
    internal class TestArtificialInteligenceExtension : ArtificialInteligenceExtension
    {
        public TestArtificialInteligenceExtension(Settings settings) 
            : base(settings)
        {
        }

        public static TimeSpan Delay = TimeSpan.FromSeconds(10);
        public static DateTimeOffset TestScheduleTime { get; private set; }
        public static string TestScheduleText { get; private set; }

        public override async Task<IEnumerable<ScheduledMessage>> GetScheduledMessagesAsync()
        {
            TestScheduleTime = DateTimeOffset.Now.Add(Delay);
            TestScheduleText = "TEST_SCHEDULED_MESSAGE";

            var result = await base.GetScheduledMessagesAsync();
            result.ForEach(s =>
            {
                s.Time = TestScheduleTime;
                s.Text = TestScheduleText;
            });
            return result;
        }
    }
}
