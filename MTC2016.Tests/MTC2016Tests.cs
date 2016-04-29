﻿using System.Threading.Tasks;
using MTC2016.Configuration;
using NUnit.Framework;
using Takenet.MessagingHub.Client.NUnitTester;

namespace MTC2016.Tests
{
    public class Mtc2016Tests : TestClass<Settings>
    {
        protected override string TesterIdentifier => Settings.TesterIdentifier;
        protected override string TesterAccessKey => Settings.TesterAccessKey;

        public override Task OneTimeSetUp()
        {
            EnableConsoleTraceListener(true);
            return base.OneTimeSetUp();
        }

        [Test]
        public async Task SubscribeWithSuccess()
        {
            await SendMessageAsync("Entrar");
            await AssertResponseAsync(Settings.Messages.ConfirmSubscription);
        }
    }
}
