using System;
using System.Linq;
using System.Threading.Tasks;
using Lime.Messaging.Contents;
using Lime.Protocol;
using MTC2016.Receivers;
using MTC2016.Tests.Mocks;
using NUnit.Framework;
using Shouldly;

namespace MTC2016.Tests
{
    [TestFixture]
    public class MessageScheduleTests : TestWith<InMemorySubscriptionAndSingleFakeScheduleServiceProvider>
    {
        [Test]
        public async Task ReceiveScheduledMessage()
        {
            var response = await Tester.ReceiveMessageAsync();
            var answer = SchedulerExtensionWithSingleFakeSchedule.TestScheduleText;
            Assert(response, answer);
        }
    }

    [TestFixture]
    public class RatingScheduleTests : TestWith<InMemorySubscriptionAndSingleFakeRatingScheduleServiceProvider>
    {
        [Test]
        public async Task ReveiveScheduledRatingRequestAndAnswerIt()
        {
            var response = await Tester.ReceiveMessageAsync();
            var ratingOptions = response.Content as Select;
            ratingOptions.ShouldNotBeNull();
            ratingOptions?.Text.ShouldStartWith(SchedulerExtensionWithSingleFakeRatingSchedule.TestScheduleText);
            ratingOptions?.Options.Length.ShouldBe(3);
            ratingOptions?.Options.First().Text.ShouldBe(Settings.BadRating);
            ratingOptions?.Options.Last().Text.ShouldBe(Settings.GoodRating);

            var currentMinute = DateTime.Now;
            var rating = ((PlainText)ratingOptions?.Options.First().Value)?.Text;
            await Tester.SendMessageAsync(rating);
            response = await Tester.ReceiveMessageAsync();
            var answer = await ApiAiForStaticContent.GetAnswerAsync(Settings.RatingConfirmation);
            Assert(response, answer);

            var from = new Node(Tester.TesterIdentifier, "msging.net", null).ToString();
            var ratingIntent = RatingMessageReceiver.CreateRatingId(Settings, from, currentMinute).Replace("_", " ");
            var storedRating = await ApiAiForStaticContent.GetAnswerAsync(ratingIntent);
            Assert(storedRating, rating?.ToLower());

            var deleted = await ApiAiForStaticContent.DeleteIntent(ratingIntent);
            Assert(deleted.ToString(), true.ToString());
        }
    }
}