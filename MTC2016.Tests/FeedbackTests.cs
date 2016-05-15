using System;
using System.Threading.Tasks;
using Lime.Protocol;
using MTC2016.Receivers;
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
