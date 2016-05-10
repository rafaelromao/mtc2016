using System;
using System.Threading.Tasks;
using Lime.Protocol;
using MTC2016.ArtificialInteligence;
using MTC2016.Receivers;
using NUnit.Framework;
using Shouldly;

namespace MTC2016.Tests
{
    [TestFixture]
    public class FeedbackTests : TestBase
    {
        [Test]
        [TestCase("feedback Gostei do Evento $15774$")]
        public async Task SendFeedback(string feedback)
        {
            var currentMinute = DateTime.Now;
            await Tester.SendMessageAsync(feedback);
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ArtificialInteligenceExtension.GetAnswerAsync(feedback);
            Assert(response, answer);

            var from = Node.Parse(Tester.TesterIdentifier).ToString();
            var feedbackId = FeedbackMessageReceiver.CreateFeedbackId(Settings, from, currentMinute);
            var storedFeedback = await ArtificialInteligenceExtension.GetIntentAsync(feedbackId);

            var deleted = await ArtificialInteligenceExtension.DeleteIntent(feedbackId);
            deleted.ShouldBeTrue();

            Assert(storedFeedback, feedback);
        }
    }
}
