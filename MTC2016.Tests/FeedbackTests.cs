using System.Threading.Tasks;
using MTC2016.Tests.Mocks;
using NUnit.Framework;

namespace MTC2016.Tests
{
    [TestFixture]
    public class FeedbackTests : TestWith<TestServiceProvider>
    {
        [Test]
        [TestCase("feedback Gostei do Evento")]
        [TestCase("#feedback Foi legal")]
        [TestCase("feedback: Muito bacana")]
        [TestCase("#feedback:É isso ai")]
        [TestCase("#feedback:012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789")]
        public async Task SendFeedback(string feedback)
        {
            await Tester.SendMessageAsync(feedback);
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ApiAiForStaticContent.GetAnswerAsync(Settings.FeedbackConfirmation);
            Assert(response, answer);

            //TODO : Assert what was stored
        }
    }
}
