using System.Threading.Tasks;
using MTC2016.Configuration;
using MTC2016.Tests.Mocks;
using NUnit.Framework;
using Takenet.MessagingHub.Client.NUnitTester;

namespace MTC2016.Tests
{
    public class SubscriptionTests : TestClass<Settings>
    {
        private async Task EnsureAlreadySubscribedAsync()
        {
            await SendMessageAsync<TestSubscribeMessageReceiver>();
            await IgnoreReceivedMessageAsync();
            await ResetAsync();
        }

        [Test]
        public async Task Subscribe()
        {
            await SendMessageAsync<TestSubscribeMessageReceiver>();
            await AssertLastMessageAsync(Settings.Messages.ConfirmSubscription);
        }

        [Test]
        public async Task SubscribeWhenAlreadySubscribed()
        {
            await EnsureAlreadySubscribedAsync();

            await SendMessageAsync<TestSubscribeMessageReceiver>();
            await AssertLastMessageAsync(Settings.Messages.AlreadySubscribed);
        }

        [Test]
        public async Task Unsubscribe()
        {
            await EnsureAlreadySubscribedAsync();

            await SendMessageAsync<TestUnsubscribeMessageReceiver>();
            await AssertLastMessageAsync(Settings.Messages.ConfirmSubscriptionCancellation);
        }

        [Test]
        public async Task UnsubscribeWhenNotSubscribed()
        {
            await SendMessageAsync<TestUnsubscribeMessageReceiver>();
            await AssertLastMessageAsync(Settings.Messages.NotSubscribed);
        }
    }
}
