using System.Threading.Tasks;
using MTC2016.Configuration;
using MTC2016.Receivers;
using MTC2016.Tests.Mocks;
using NUnit.Framework;
using Shouldly;
using Takenet.MessagingHub.Client.Tester;

namespace MTC2016.Tests
{
    [TestFixture]
    public class SubscriptionTests
    {
        private ClientTester<Settings> _tester;

        [SetUp]
        public void SetUp()
        {
            _tester = new ClientTester<Settings>(new ClientTesterOptions
            {
                TestServiceProviderType = typeof(TestServiceProvider)
            });
        }

        [TearDown]
        public void TearDown()
        {
            _tester.Dispose();
        }

        private async Task EnsureAlreadySubscribedAsync()
        {
            await _tester.SendMessageAsync<SubscribeMessageReceiver>();
            await _tester.IgnoreMessageAsync();
            await _tester.ResetAsync();
        }
        [Test]
        public async Task Subscribe()
        {
            await _tester.SendMessageAsync<SubscribeMessageReceiver>();
            var response = await _tester.ReceiveMessageAsync();
            response?.Content?.ToString().ShouldBe(_tester.Settings.Messages.ConfirmSubscription);
        }

        [Test]
        public async Task SubscribeWhenAlreadySubscribed()
        {
            await EnsureAlreadySubscribedAsync();

            await _tester.SendMessageAsync<SubscribeMessageReceiver>();
            var response = await _tester.ReceiveMessageAsync();
            response?.Content?.ToString().ShouldBe(_tester.Settings.Messages.AlreadySubscribed);
        }

        [Test]
        public async Task Unsubscribe()
        {
            await EnsureAlreadySubscribedAsync();

            await _tester.SendMessageAsync<UnsubscribeMessageReceiver>();
            var response = await _tester.ReceiveMessageAsync();
            response?.Content?.ToString().ShouldBe(_tester.Settings.Messages.ConfirmSubscriptionCancellation);
        }

        [Test]
        public async Task UnsubscribeWhenNotSubscribed()
        {
            await _tester.SendMessageAsync<UnsubscribeMessageReceiver>();
            var response = await _tester.ReceiveMessageAsync();
            response?.Content?.ToString().ShouldBe(_tester.Settings.Messages.NotSubscribed);
        }
    }
}
