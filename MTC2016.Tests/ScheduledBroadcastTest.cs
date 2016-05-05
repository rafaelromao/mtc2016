using System.Threading.Tasks;
using MTC2016.Receivers;
using MTC2016.Tests.Mocks;
using NUnit.Framework;

namespace MTC2016.Tests
{
    [TestFixture]
    public class ScheduledBroadcastTests : TestBase
    {
        [Test]
        public async Task ScheduledBroadcastIsExecuted()
        {
            await Tester.SendMessageAsync<SubscribeMessageReceiver>();
            await Tester.IgnoreMessageAsync();

            // When intantiated, TestArtificialInteligenceExtension will reschedule all messages to the instantiation time plus Delay,
            // So it shall be scheduled when the Smart Contact is instantiated (TestBase.Setup) and the Tester should then receive the message
            await Task.Delay(TestArtificialInteligenceExtension.Delay);

            var response = await Tester.ReceiveMessageAsync();
            var answer = TestArtificialInteligenceExtension.TestScheduleText;
            Assert(response, answer);
        }
    }
}
