using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lime.Messaging.Contents;
using Lime.Protocol;
using Lime.Protocol.Serialization;
using MTC2016.ArtificialInteligence;
using MTC2016.Configuration;
using MTC2016.DistributionList;
using MTC2016.Scheduler;
using Takenet.MessagingHub.Client.Sender;

namespace MTC2016.Tests.Mocks
{
    internal class SchedulerExtensionWithSingleFakeSchedule : SchedulerExtension
    {
        public static DateTimeOffset TestScheduleTime { get; protected set; }
        public static string TestScheduleText { get; } = "TEST_SCHEDULED_MESSAGE";

        private readonly IDistributionListExtension _distributionListExtension;

        public SchedulerExtensionWithSingleFakeSchedule(IArtificialInteligenceExtension artificialInteligenceExtension,
            IDistributionListExtension distributionListExtension,
            IJobScheduler jobScheduler, IMessagingHubSender sender, Settings settings)
            : base(artificialInteligenceExtension, jobScheduler, sender, settings)
        {
            _distributionListExtension = distributionListExtension;
        }

        public override async Task ScheduleAsync(Func<Task<IEnumerable<Identity>>> recipients,
            IEnumerable<ScheduledMessage> scheduledMessages, CancellationToken cancellationToken)
        {
            await _distributionListExtension.AddAsync(Identity.Parse("mtc2016$tester@msging.net"), cancellationToken);

            await base.ScheduleAsync(recipients, scheduledMessages, cancellationToken);
        }

        public override async Task<IEnumerable<ScheduledMessage>> GetScheduledMessagesAsync(
            CancellationToken cancellationToken)
        {
            TestScheduleTime = DateTimeOffset.Now;

            var result = await base.GetScheduledMessagesAsync(CancellationToken.None);
            result = result.Take(1);
            result.ForEach(s =>
            {
                s.Time = TestScheduleTime;
                s.Message = new PlainText { Text = TestScheduleText };
            });
            return result;
        }
    }
}
