using System;
using System.Threading.Tasks;
using MTC2016.Receivers;
using MTC2016.Tests.Mocks;
using NUnit.Framework;

namespace MTC2016.Tests
{
    [TestFixture]
    public class SubscriptionTests : TestBase<FakeScheduleAndSubscriptionTestsServiceProvider>
    {
        private async Task EnsureAlreadySubscribedAsync()
        {
            await Tester.SendMessageAsync<SubscribeMessageReceiver>();
            await Tester.IgnoreMessageAsync(TimeSpan.FromSeconds(2));
        }
        private async Task EnsureNotSubscribedAsync()
        {
            await Tester.SendMessageAsync<UnsubscribeMessageReceiver>();
            await Tester.IgnoreMessageAsync(TimeSpan.FromSeconds(2));
        }


        [Test]
        public async Task Subscribe()
        {
            await EnsureNotSubscribedAsync();

            await Tester.SendMessageAsync<SubscribeMessageReceiver>();
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ArtificialInteligenceExtension.GetAnswerAsync(Settings.ConfirmSubscription);
            Assert(response, answer);
        }

        [Test]
        public async Task SubscribeConversationally()
        {
            await EnsureNotSubscribedAsync();

            await Tester.SendMessageAsync("Olá, quero me inscrever!");
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ArtificialInteligenceExtension.GetAnswerAsync(Settings.ConfirmSubscription);
            Assert(response, answer);
        }


        [Test]
        public async Task SubscribeWhenAlreadySubscribed()
        {
            await EnsureAlreadySubscribedAsync();

            await Tester.SendMessageAsync<SubscribeMessageReceiver>();
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ArtificialInteligenceExtension.GetAnswerAsync(Settings.AlreadySubscribed);
            Assert(response, answer);
        }

        [Test]
        public async Task Unsubscribe()
        {
            await EnsureAlreadySubscribedAsync();

            await Tester.SendMessageAsync<UnsubscribeMessageReceiver>();
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ArtificialInteligenceExtension.GetAnswerAsync(Settings.ConfirmSubscriptionCancellation);
            Assert(response, answer);
        }

        [Test]
        public async Task UnsubscribeWhenNotSubscribed()
        {
            await EnsureNotSubscribedAsync();

            await Tester.SendMessageAsync<UnsubscribeMessageReceiver>();
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ArtificialInteligenceExtension.GetAnswerAsync(Settings.NotSubscribed);
            Assert(response, answer);
        }
    }
}
