using MTC2016.ArtificialInteligence;
using MTC2016.Configuration;
using MTC2016.DistributionList;
using MTC2016.Scheduler;
using Takenet.MessagingHub.Client.Tester;

namespace MTC2016.Tests.Mocks
{
    public class TestsServiceProvider : ApplicationTesterServiceProvider
    {
        public static void RegisterTestService<TInterface, TClass>()
            where TInterface : class
            where TClass : class, TInterface
        {
            ((ServiceProvider)ApplicationTester.ApplicationServiceProvider).Container.RegisterSingleton<TInterface, TClass>();
        }
    }

    public class FakeScheduleAndSubscriptionTestsServiceProvider : TestsServiceProvider
    {
        static FakeScheduleAndSubscriptionTestsServiceProvider()
        {
            RegisterTestService<IDistributionListExtension, TestDistributionListExtension>();
            RegisterTestService<ISchedulerExtension, TestSchedulerExtension>();
        }
    }
}
