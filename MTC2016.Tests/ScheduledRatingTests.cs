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
    public class ScheduledRatingTests : TestBase<FakeRatingScheduleAndSubscriptionTestsServiceProvider>
    {
        [Test]
        public async Task ScheduledRatingIsExecuted()
        {
            var response = await Tester.ReceiveMessageAsync();
            var ratingOptions = response.Content as Select;
            ratingOptions.ShouldNotBeNull();
            ratingOptions?.Text.ShouldStartWith(RatingTestSchedulerExtension.TestScheduleText);
            ratingOptions?.Options.Length.ShouldBe(5);
            ratingOptions?.Options.First().Text.ShouldBe(Settings.PrettyBadRating);
            ratingOptions?.Options.Last().Text.ShouldBe(Settings.PrettyGoodRating);

            var currentMinute = DateTime.Now;
            var rating = ((PlainText)ratingOptions?.Options.First().Value)?.Text;
            await Tester.SendMessageAsync(rating);
            response = await Tester.ReceiveMessageAsync();
            var answer = await ArtificialInteligenceExtension.GetAnswerAsync(Settings.RatingConfirmation);
            Assert(response, answer);

            var from = new Node(Tester.TesterIdentifier, "msging.net", null).ToString();
            var ratingIntent = RatingMessageReceiver.CreateRatingId(Settings, from, currentMinute).Replace("_", " ");
            var storedRating = await ArtificialInteligenceExtension.GetAnswerAsync(ratingIntent);
            Assert(storedRating, rating?.ToLower());

            var deleted = await ArtificialInteligenceExtension.DeleteIntent(ratingIntent);
            Assert(deleted.ToString(), true.ToString());
        }
    }
}