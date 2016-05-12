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
    internal class RatingTestSchedulerExtension : SchedulerExtension
    {
        public static DateTimeOffset TestScheduleTime { get; protected set; }
        public static string TestScheduleText { get; set; }

        private readonly IDistributionListExtension _distributionListExtension;

        public RatingTestSchedulerExtension(IArtificialInteligenceExtension artificialInteligenceExtension,
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

        public override async Task<IEnumerable<ScheduledMessage>> GetScheduledMessagesAsync(CancellationToken cancellationToken)
        {
            TestScheduleTime = DateTimeOffset.Now;

            var result = await base.GetScheduledMessagesAsync(CancellationToken.None);
            result = result.Where(s => s.Message is Select).Take(1);
            result.ForEach(s =>
            {
                TestScheduleText = ((Select) s.Message).Text;
                s.Time = TestScheduleTime;
            });
            return result;
        }
    }
}