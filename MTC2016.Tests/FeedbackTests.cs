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
            var currentMinute = DateTime.Now;
            await Tester.SendMessageAsync(feedback);
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ApiAi.GetAnswerAsync(Settings.FeedbackConfirmation);
            Assert(response, answer);

            var from = new Node(Tester.TesterIdentifier, "msging.net", null).ToString();
            var feedbackIntent = FeedbackMessageReceiver.CreateFeedbackId(Settings, from, currentMinute).Replace("_", " ");
            var storedFeedback = await ApiAi.GetAnswerAsync(feedbackIntent);
            Assert(storedFeedback, feedback.Replace("#", ""));

            var deleted = await ApiAi.DeleteIntent(feedbackIntent);
            Assert(deleted.ToString(), true.ToString());
        }
    }
}
