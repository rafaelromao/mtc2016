using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lime.Messaging.Contents;
using Lime.Protocol;
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

        public SchedulerExtensionWithSingleFakeSchedule(IApiAiForStaticContent apiAi,
            IDistributionListExtension distributionListExtension,
            IJobScheduler jobScheduler, IRecipientsRepository recipientsRepository, Settings settings)
            : base(apiAi, jobScheduler, recipientsRepository, settings)
        {
            _distributionListExtension = distributionListExtension;
        }

        protected override async Task ScheduleAsync(IEnumerable<ScheduledMessage> messagesToBeScheduled, CancellationToken cancellationToken)
        {
            await _distributionListExtension.AddAsync(Identity.Parse("mtc2016$tester@msging.net"), cancellationToken);

            await base.ScheduleAsync(messagesToBeScheduled, cancellationToken);
        }

        protected override async Task<IEnumerable<ScheduledMessage>> GetMessagesToBeScheduled(CancellationToken cancellationToken)
        {
            TestScheduleTime = DateTimeOffset.Now;

            var result = await base.GetMessagesToBeScheduled(CancellationToken.None);
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
