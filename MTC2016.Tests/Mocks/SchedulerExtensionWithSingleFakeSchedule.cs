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

namespace MTC2016.Tests.Mocks
{
    internal class SchedulerExtensionWithSingleFakeSchedule : SchedulerExtension
    {
        public static string TestScheduleText { get; } = "TEST_SCHEDULED_MESSAGE";

        private readonly IDistributionListExtension _distributionListExtension;

        public SchedulerExtensionWithSingleFakeSchedule(IApiAiForStaticContent apiAi,
            IDistributionListExtension distributionListExtension,
            IJobScheduler jobScheduler, IRecipientsRepository recipientsRepository, Settings settings)
            : base(apiAi, jobScheduler, recipientsRepository, settings)
        {
            _distributionListExtension = distributionListExtension;
        }

        protected override async Task ScheduleAsync(IEnumerable<ScheduledMessage> messagesToBeScheduled)
        {
            await _distributionListExtension.AddAsync(Identity.Parse("mtc2016$tester@msging.net"), CancellationToken.None);

            await base.ScheduleAsync(messagesToBeScheduled);
        }

        protected override async Task<IEnumerable<ScheduledMessage>> GetMessagesToBeScheduled()
        {
            var result = await base.GetMessagesToBeScheduled();
            result = result.Take(1);
            result.ForEach(s =>
            {
                s.Time = DateTimeOffset.Now;
                s.Message = new PlainText { Text = TestScheduleText };
            });
            return result;
        }
    }
}
