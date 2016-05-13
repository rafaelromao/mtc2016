using System;
using System.Threading.Tasks;
using MTC2016.Receivers;
using MTC2016.Tests.Mocks;
using NUnit.Framework;

namespace MTC2016.Tests
{
    [TestFixture]
    public class InMemorySubscribeTests : TestWith<InMemorySubscriptionServiceProvider>
    {
        [Test]
        public async Task SubscribeWhenAlreadySubscribed()
        {
            await Tester.SendMessageAsync("Olá, quero me inscrever!");
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ArtificialInteligenceExtension.GetAnswerAsync(Settings.ConfirmSubscription);
            Assert(response, answer);

            await Tester.SendMessageAsync("entrar");
            response = await Tester.ReceiveMessageAsync();
            answer = await ArtificialInteligenceExtension.GetAnswerAsync(Settings.AlreadySubscribed);
            Assert(response, answer);
        }
    }

    [TestFixture]
    public class InMemoryUnsubscribeTests : TestWith<InMemorySubscriptionServiceProvider>
    {
        [Test]
        public async Task Unsubscribe()
        {
            await Tester.SendMessageAsync("Olá, quero me inscrever!");
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ArtificialInteligenceExtension.GetAnswerAsync(Settings.ConfirmSubscription);
            Assert(response, answer);

            await Tester.SendMessageAsync("Prefiro cancelar");
            response = await Tester.ReceiveMessageAsync();
            answer = await ArtificialInteligenceExtension.GetAnswerAsync(Settings.ConfirmSubscriptionCancellation);
            Assert(response, answer);
        }

    }

    [TestFixture]
    public class NoPersistentSubscriptionTests : TestWith<NoSubscriptionAndSingleFakeScheduleServiceProvider>
    {
        [Test]
        public async Task Subscribe()
        {
            await Tester.SendMessageAsync("Gostaria de participar!");
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ArtificialInteligenceExtension.GetAnswerAsync(Settings.ConfirmSubscription);
            Assert(response, answer);
        }

        [Test]
        public async Task UnsubscribeWhenNotSubscribed()
        {
            await Tester.SendMessageAsync("desistir!");
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ArtificialInteligenceExtension.GetAnswerAsync(Settings.NotSubscribed);
            Assert(response, answer);
        }
    }
}