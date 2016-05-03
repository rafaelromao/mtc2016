using System.Threading.Tasks;
using MTC2016.Receivers;
using NUnit.Framework;

namespace MTC2016.Tests
{
    [TestFixture]
    public class SubscriptionTests : TestBase
    {
        private async Task EnsureAlreadySubscribedAsync()
        {
            await Tester.SendMessageAsync<SubscribeMessageReceiver>();
            await Tester.IgnoreMessagesAsync();
        }
        [Test]
        public async Task Subscribe()
        {
            await Tester.SendMessageAsync<SubscribeMessageReceiver>();
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ArtificialInteligenceExtension.GetAnswerAsync(Tester.Settings.ConfirmSubscription);
            Assert(response, answer);
        }

        [Test]
        public async Task SubscribeWhenAlreadySubscribed()
        {
            await EnsureAlreadySubscribedAsync();

            await Tester.SendMessageAsync<SubscribeMessageReceiver>();
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ArtificialInteligenceExtension.GetAnswerAsync(Tester.Settings.AlreadySubscribed);
            Assert(response, answer);
        }

        [Test]
        public async Task Unsubscribe()
        {
            await EnsureAlreadySubscribedAsync();

            await Tester.SendMessageAsync<UnsubscribeMessageReceiver>();
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ArtificialInteligenceExtension.GetAnswerAsync(Tester.Settings.ConfirmSubscriptionCancellation);
            Assert(response, answer);
        }

        [Test]
        public async Task UnsubscribeWhenNotSubscribed()
        {
            await Tester.SendMessageAsync<UnsubscribeMessageReceiver>();
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ArtificialInteligenceExtension.GetAnswerAsync(Tester.Settings.NotSubscribed);
            Assert(response, answer);
        }
    }
}
