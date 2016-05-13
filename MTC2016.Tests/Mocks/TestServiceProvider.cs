using MTC2016.Configuration;
using MTC2016.DistributionList;
using MTC2016.Receivers;
using MTC2016.Scheduler;
using Takenet.MessagingHub.Client.Tester;

namespace MTC2016.Tests.Mocks
{
    public class TestServiceProvider : ApplicationTesterServiceProvider
    {
        public static void RegisterTestService<TInterface, TClass>()
            where TInterface : class
            where TClass : class, TInterface
        {
            ((ServiceProvider)ApplicationTester.ApplicationServiceProvider).Container.RegisterSingleton<TInterface, TClass>();
        }
    }

    public class InMemorySubscriptionAndSingleFakeScheduleServiceProvider : TestServiceProvider
    {
        static InMemorySubscriptionAndSingleFakeScheduleServiceProvider()
        {
            RegisterTestService<IDistributionListExtension, InMemoryDistributionListExtension>();
            RegisterTestService<ISchedulerExtension, SchedulerExtensionWithSingleFakeSchedule>();
        }
    }

    public class NoSubscriptionAndSingleFakeScheduleServiceProvider : TestServiceProvider
    {
        static NoSubscriptionAndSingleFakeScheduleServiceProvider()
        {
            RegisterTestService<IDistributionListExtension, DummyTestDistributionListExtension>();
            RegisterTestService<ISchedulerExtension, SchedulerExtensionWithSingleFakeSchedule>();
        }
    }

    public class InMemorySubscriptionServiceProvider : TestServiceProvider
    {
        static InMemorySubscriptionServiceProvider()
        {
            RegisterTestService<IDistributionListExtension, InMemoryDistributionListExtension>();
        }
    }

    public class InMemorySubscriptionAndSingleFakeRatingScheduleServiceProvider : TestServiceProvider
    {
        static InMemorySubscriptionAndSingleFakeRatingScheduleServiceProvider()
        {
            RegisterTestService<IDistributionListExtension, InMemoryDistributionListExtension>();
            RegisterTestService<ISchedulerExtension, SchedulerExtensionWithSingleFakeRatingSchedule>();
        }
    }

    public class QuestionMessageReceiverThatDoesNotParseImagesServiceProvider : TestServiceProvider
    {
        static QuestionMessageReceiverThatDoesNotParseImagesServiceProvider()
        {
            RegisterTestService<QuestionMessageReceiver, QuestionMessageReceiverThatDoesNotParseImages>();
        }
    }
}
