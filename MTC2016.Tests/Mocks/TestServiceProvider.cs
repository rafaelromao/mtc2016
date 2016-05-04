using System;
using MTC2016.ArtificialInteligence;
using MTC2016.Configuration;
using MTC2016.DistributionList;
using MTC2016.Scheduler;
using Takenet.MessagingHub.Client.Tester;

namespace MTC2016.Tests.Mocks
{
    public class TestServiceProvider : ApplicationTesterServiceProvider
    {
        public TestServiceProvider()
        {
            RegisterTestService<IDistributionListExtension, TestDistributionListExtension>();
            RegisterTestService<ISchedulerExtension, TestSchedulerExtension>();
            RegisterTestService<IJobScheduler, TestJobScheduler>();
            RegisterTestService<IArtificialInteligenceExtension, TestArtificialInteligenceExtension>();
        }

        public sealed override void RegisterTestService<TInterface, TClass>()
        {
            ((ServiceProvider)ApplicationTester.ApplicationServiceProvider).Container.Register<TInterface, TClass>();
        }
    }
}
