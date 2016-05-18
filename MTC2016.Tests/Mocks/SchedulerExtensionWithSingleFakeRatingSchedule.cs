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
    internal class SchedulerExtensionWithSingleFakeRatingSchedule : SchedulerExtension
    {
        public static string TestScheduleText { get; set; }

        private readonly IDistributionListExtension _distributionListExtension;

        public SchedulerExtensionWithSingleFakeRatingSchedule(IApiAiForStaticContent apiAi,
            IDistributionListExtension distributionListExtension, IJobScheduler jobScheduler, Settings settings)
            : base(apiAi, jobScheduler, distributionListExtension, settings)
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
            result = result.Where(s => s.Message is Select).Take(1);
            result.ForEach(s =>
            {
                TestScheduleText = ((Select) s.Message).Text;
                s.Time = DateTimeOffset.Now;
            });
            return result;
        }
    }
}