using System.Linq;
using System.Threading.Tasks;
using Lime.Messaging.Contents;
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
            var ratingOptions = response?.Content as Select;
            ratingOptions.ShouldNotBeNull();
            ratingOptions?.Text.ShouldStartWith(SchedulerExtensionWithSingleFakeRatingSchedule.TestScheduleText);
            ratingOptions?.Options.Length.ShouldBe(3);
            ratingOptions?.Options.First().Text.ShouldBe(Settings.BadRating);
            ratingOptions?.Options.Last().Text.ShouldBe(Settings.GoodRating);

            var rating = ((PlainText)ratingOptions?.Options.First().Value)?.Text;
            await Tester.SendMessageAsync(rating);
            response = await Tester.ReceiveMessageAsync();
            var answer = await ApiAiForStaticContent.GetAnswerAsync(Settings.RatingConfirmation);
            Assert(response, answer);

            //TODO : Assert what was stored
        }
    }
}