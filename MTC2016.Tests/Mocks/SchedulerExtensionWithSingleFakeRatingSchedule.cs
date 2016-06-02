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
            await
                _distributionListExtension.AddAsync(Identity.Parse("mtc2016$tester@msging.net"), CancellationToken.None);

            await base.ScheduleAsync(messagesToBeScheduled);
        }

        protected override async Task<IEnumerable<ScheduledMessage>> GetMessagesToBeScheduled()
        {
            var result = await base.GetMessagesToBeScheduled();
            result = result.Where(s => s.DefaultMessage is Select).Take(1);
            result.ForEach(s =>
            {
                TestScheduleText = ((Select) s.DefaultMessage).Text;
                s.Time = DateTimeOffset.Now;
            });
            return result;
        }
    }

    internal class SchedulerExtensionWithSingleFakeOmniRatingSchedule : SchedulerExtensionWithSingleFakeRatingSchedule
    {
        public SchedulerExtensionWithSingleFakeOmniRatingSchedule(
            IApiAiForStaticContent apiAi, IDistributionListExtension distributionListExtension, 
            IJobScheduler jobScheduler, Settings settings) 
            : base(apiAi, distributionListExtension, jobScheduler, settings)
        {
        }

        protected override string GetDomain(Identity domain)
        {
            return Domains.Omni;
        }

        protected override async Task<IEnumerable<ScheduledMessage>> GetMessagesToBeScheduled()
        {
            var result = await base.GetMessagesToBeScheduled();
            result = result.Where(s => s.DefaultMessage is Select && s.OmniMessage != s.DefaultMessage).Take(1);
            result.ForEach(s =>
            {
                TestScheduleText = ((PlainText)s.OmniMessage).Text;
                s.Time = DateTimeOffset.Now;
            });
            return result;
        }
    }

    internal class SchedulerExtensionWithSingleFakeTangramRatingSchedule : SchedulerExtensionWithSingleFakeRatingSchedule
    {
        public SchedulerExtensionWithSingleFakeTangramRatingSchedule(
            IApiAiForStaticContent apiAi, IDistributionListExtension distributionListExtension,
            IJobScheduler jobScheduler, Settings settings)
            : base(apiAi, distributionListExtension, jobScheduler, settings)
        {
        }

        protected override string GetDomain(Identity domain)
        {
            return Domains.Tangram;
        }

        protected override async Task<IEnumerable<ScheduledMessage>> GetMessagesToBeScheduled()
        {
            var result = await base.GetMessagesToBeScheduled();
            result = result.Where(s => s.DefaultMessage is Select && s.TangramMessage != s.DefaultMessage).Take(1);
            result.ForEach(s =>
            {
                TestScheduleText = ((PlainText)s.TangramMessage).Text;
                s.Time = DateTimeOffset.Now;
            });
            return result;
        }
    }
}