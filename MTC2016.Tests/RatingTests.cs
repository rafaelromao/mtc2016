using System.Linq;
using System.Threading.Tasks;
using Lime.Messaging.Contents;
using MTC2016.Tests.Mocks;
using NUnit.Framework;
using Shouldly;

namespace MTC2016.Tests
{
    [TestFixture]
    public class RatingTests : TestClass<InMemorySubscriptionAndSingleFakeRatingScheduleServiceProvider>
    {
        [Test]
        public async Task ReveiveScheduledRatingRequestAndAnswerIt()
        {
            var response = await Tester.ReceiveMessageAsync();
            var ratingOptions = response?.Content as Select;
            ratingOptions.ShouldNotBeNull();
            ratingOptions?.Text.ShouldStartWith(SchedulerExtensionWithSingleFakeRatingSchedule.TestScheduleText);
            ratingOptions?.Options.Length.ShouldBe(3);

            var badRatingsIntent = await ApiAiForStaticContent.GetIntentAsync(Settings.BadRating);
            var badRatings = badRatingsIntent.Responses.First().Speech;
            Assert(ratingOptions?.Options.First().Text, badRatings);

            var goodRatingsIntent = await ApiAiForStaticContent.GetIntentAsync(Settings.GoodRating);
            var goodRatings = goodRatingsIntent.Responses.First().Speech;
            Assert(ratingOptions?.Options.Last().Text, goodRatings);

            var rating = ((PlainText)ratingOptions?.Options.First().Value)?.Text;
            await Tester.SendMessageAsync(rating);
            response = await Tester.ReceiveMessageAsync();
            var answer = await ApiAiForStaticContent.GetAnswerAsync(Settings.RatingConfirmation);
            Assert(response, answer);

            //TODO : Assert what was stored
        }
    }

    [TestFixture]
    public class OmniRatingTests : TestClass<InMemorySubscriptionAndSingleFakeOmniRatingScheduleServiceProvider>
    {
        [Test]
        public async Task ReveiveScheduledRatingRequestAndAnswerIt()
        {
            var response = await Tester.ReceiveMessageAsync();
            var ratingOptions = response?.Content as PlainText;
            ratingOptions.ShouldNotBeNull();
            ratingOptions?.Text.ShouldStartWith(SchedulerExtensionWithSingleFakeRatingSchedule.TestScheduleText);
            ratingOptions?.Text.ShouldContain("Escolha:");
        }
    }

    [TestFixture]
    public class TangramRatingTests : TestClass<InMemorySubscriptionAndSingleFakeTangramRatingScheduleServiceProvider>
    {
        [Test]
        public async Task ReveiveScheduledRatingRequestAndAnswerIt()
        {
            var response = await Tester.ReceiveMessageAsync();
            var ratingOptions = response?.Content as Select;
            ratingOptions.ShouldNotBeNull();
            ratingOptions?.Text.ShouldStartWith(SchedulerExtensionWithSingleFakeRatingSchedule.TestScheduleText);

            var badRatingsIntent = await ApiAiForStaticContent.GetIntentAsync(Settings.BadRating);
            var badRatings = badRatingsIntent.Responses.First().Speech;
            Assert(ratingOptions?.Options.First().Text, badRatings);

            var goodRatingsIntent = await ApiAiForStaticContent.GetIntentAsync(Settings.GoodRating);
            var goodRatings = goodRatingsIntent.Responses.First().Speech;
            Assert(ratingOptions?.Options.Last().Text, goodRatings);
        }
    }
}