using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Lime.Protocol.Serialization;
using MTC2016.ArtificialInteligence;
using MTC2016.Configuration;
using MTC2016.DistributionList;
using MTC2016.Scheduler;
using Takenet.MessagingHub.Client.Sender;

namespace MTC2016.Tests.Mocks
{
    internal class TestSchedulerExtension : SchedulerExtension
    {
        private readonly IDistributionListExtension _distributionListExtension;

        public TestSchedulerExtension(IArtificialInteligenceExtension artificialInteligenceExtension, 
            IDistributionListExtension distributionListExtension,
            IJobScheduler jobScheduler, IMessagingHubSender sender, Settings settings) 
            : base(artificialInteligenceExtension, jobScheduler, sender, settings)
        {
            _distributionListExtension = distributionListExtension;
        }

        public static DateTimeOffset TestScheduleTime { get; private set; }
        public static string TestScheduleText { get; private set; }

        public override async Task ScheduleAsync(Func<Task<IEnumerable<Identity>>> recipients, IEnumerable<ScheduledMessage> scheduledMessages, CancellationToken cancellationToken)
        {
            await _distributionListExtension.AddAsync(Identity.Parse("mtc2016$tester@msging.net"), cancellationToken);

            await base.ScheduleAsync(recipients, scheduledMessages, cancellationToken);
        }

        public override async Task<IEnumerable<ScheduledMessage>> GetScheduledMessagesAsync(CancellationToken cancellationToken)
        {
            TestScheduleTime = DateTimeOffset.Now;
            TestScheduleText = "TEST_SCHEDULED_MESSAGE";

            var result = await base.GetScheduledMessagesAsync(CancellationToken.None);
            result.ForEach(s =>
            {
                s.Time = TestScheduleTime;
                s.Text = TestScheduleText;
            });
            return result;
        }
    }
}
