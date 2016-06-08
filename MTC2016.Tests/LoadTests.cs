using System;
using System.Threading.Tasks;
using MTC2016.Tests.Mocks;
using NUnit.Framework;

namespace MTC2016.Tests
{
    [TestFixture]
    public class LoadTests : TestClass<NoSubscriptionAndSingleFakeScheduleServiceProvider>
    {
        [Test]
        public async Task FiveHundredSubscriptions()
        {
            const int messageCount = 60;
            const int testerCount = 60;
            await LoadTester.PrepareTestersAsync(testerCount);
            Console.WriteLine($"ALL {testerCount} TESTERS REGISTERED!");
            await LoadTester.SendMessagesAsync("Gostaria de participar!", messageCount, testerCount);
            var responses = await LoadTester.ReceiveMessagesAsync(TimeSpan.FromSeconds(testerCount / 2), TimeSpan.FromSeconds(0.5));
            var answer = await ApiAiForStaticContent.GetAnswerAsync(Settings.ConfirmSubscription);
            Assert(responses, answer, messageCount, testerCount);
        }
    }
}