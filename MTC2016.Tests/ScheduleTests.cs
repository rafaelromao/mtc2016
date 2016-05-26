using System.Threading.Tasks;
using MTC2016.Tests.Mocks;
using NUnit.Framework;

namespace MTC2016.Tests
{
    [TestFixture]
    public class MessageScheduleTests : TestClass<InMemorySubscriptionAndSingleFakeScheduleServiceProvider>
    {
        [Test]
        public async Task ReceiveScheduledMessage()
        {
            var response = await Tester.ReceiveMessageAsync();
            var answer = SchedulerExtensionWithSingleFakeSchedule.TestScheduleText;
            Assert(response, answer);
        }
    }
}