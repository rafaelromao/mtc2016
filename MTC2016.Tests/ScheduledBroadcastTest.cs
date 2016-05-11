using System;
using System.Threading.Tasks;
using MTC2016.Receivers;
using MTC2016.Tests.Mocks;
using NUnit.Framework;

namespace MTC2016.Tests
{
    [TestFixture]
    public class ScheduledBroadcastTests : TestBase<FakeScheduleAndSubscriptionTestsServiceProvider>
    {
        [Test]
        public async Task ScheduledBroadcastIsExecuted()
        {
            var response = await Tester.ReceiveMessageAsync();
            var answer = TestSchedulerExtension.TestScheduleText;
            Assert(response, answer);
        }
    }
}
