using System.Threading.Tasks;
using MTC2016.Configuration;
using MTC2016.Receivers;
using NUnit.Framework;
using Takenet.MessagingHub.Client.NUnitTester;

namespace MTC2016.Tests
{
    public class SubscriptionTests : TestClass<Settings>
    {
        private async Task EnsureNotSubscribedAsync()
        {
            await SendMessageForReceiverAsync<UnsubscribeMessageReceiver>();
            await ResetAsync();
        }

        [Test]
        public async Task Subscribe()
        {
            await EnsureNotSubscribedAsync();

            await SendMessageForReceiverAsync<SubscribeMessageReceiver>();
            await AssertLastReceivedMessageAsync(Settings.Messages.ConfirmSubscription);
        }

        [Test]
        public async Task SubscribeWhenAlreadySubscribed()
        {
            await EnsureNotSubscribedAsync();

            await SendMessageForReceiverAsync<SubscribeMessageReceiver>();
            await IgnoreReceivedMessageAsync();
            await SendMessageForReceiverAsync<SubscribeMessageReceiver>();
            await AssertLastReceivedMessageAsync(Settings.Messages.SubscriptionFailed);
        }

        [Test]
        public async Task Unsubscribe()
        {
            await EnsureNotSubscribedAsync();

            await SendMessageForReceiverAsync<SubscribeMessageReceiver>();
            await IgnoreReceivedMessageAsync();
            await SendMessageForReceiverAsync<UnsubscribeMessageReceiver>();
            await AssertLastReceivedMessageAsync(Settings.Messages.ConfirmSubscriptionCancellation);
        }

        [Test]
        public async Task UnsubscribeWhenNotSubscribed()
        {
            await EnsureNotSubscribedAsync();

            await SendMessageForReceiverAsync<UnsubscribeMessageReceiver>();
            await AssertLastReceivedMessageAsync(Settings.Messages.NotSubscribed);
        }
    }
}
