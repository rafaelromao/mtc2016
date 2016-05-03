using MTC2016.ArtificialInteligence;
using MTC2016.DistributionList;
using MTC2016.Scheduler;
using Takenet.MessagingHub.Client.Tester;

namespace MTC2016.Tests.Mocks
{
    public class TestServiceProvider : ApplicationTesterServiceProvider
    {
        public TestServiceProvider() 
        {
            RegisterService(typeof(IDistributionListExtension), new TestDistributionListExtension());
            RegisterService(typeof(ISchedulerExtension), new TestSchedulerExtension());
        }
    }
}
