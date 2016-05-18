﻿using System;
using System.Threading.Tasks;
using MTC2016.Receivers;
using MTC2016.Tests.Mocks;
using NUnit.Framework;

namespace MTC2016.Tests
{
    [TestFixture]
    public class RealSubscribeTests : TestWith<TestServiceProvider>
    {
        [Test]
        public async Task SubscribeUnsubscribeAndSubscribeTwiceAgain()
        {
            await Tester.SendMessageAsync("Gostaria de participar!");
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ApiAiForStaticContent.GetAnswerAsync(Settings.ConfirmSubscription);
            Assert(response, answer);

            await Tester.SendMessageAsync("Mudei de ideia, quero cancelar");
            response = await Tester.ReceiveMessageAsync();
            answer = await ApiAiForStaticContent.GetAnswerAsync(Settings.ConfirmSubscriptionCancellation);
            Assert(response, answer);

            await Tester.SendMessageAsync("entrar agora");
            response = await Tester.ReceiveMessageAsync();
            answer = await ApiAiForStaticContent.GetAnswerAsync(Settings.ConfirmSubscription);
            Assert(response, answer);

            await Tester.SendMessageAsync("inscrever");
            response = await Tester.ReceiveMessageAsync();
            answer = await ApiAiForStaticContent.GetAnswerAsync(Settings.AlreadySubscribed);
            Assert(response, answer);
        }
    }

    [TestFixture]
    public class EveryoneIsAlreadySubscribedSubscribeTests : TestWith<EveryoneIsSubscribedSubscriptionServiceProvider>
    {
        [Test]
        public async Task SubscribeWhenAlreadySubscribed()
        {
            await Tester.SendMessageAsync("entrar");
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ApiAiForStaticContent.GetAnswerAsync(Settings.AlreadySubscribed);
            Assert(response, answer);
        }

        [Test]
        public async Task Unsubscribe()
        {
            await Tester.SendMessageAsync("Mudei de idéia, quero cancelar");
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ApiAiForStaticContent.GetAnswerAsync(Settings.ConfirmSubscriptionCancellation);
            Assert(response, answer);
        }

    }

    [TestFixture]
    public class NoOneIsAlreadySubscribedSubscriptionTests : TestWith<NoSubscriptionAndSingleFakeScheduleServiceProvider>
    {
        [Test]
        public async Task Subscribe()
        {
            await Tester.SendMessageAsync("Gostaria de participar!");
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ApiAiForStaticContent.GetAnswerAsync(Settings.ConfirmSubscription);
            Assert(response, answer);
        }

        [Test]
        public async Task UnsubscribeWhenNotSubscribed()
        {
            await Tester.SendMessageAsync("desistir!");
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ApiAiForStaticContent.GetAnswerAsync(Settings.NotSubscribed);
            Assert(response, answer);
        }
    }
}