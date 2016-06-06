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
            const int messageCount = 500;
            const int testerCount = 500;
            await LoadTester.PrepareTestersAsync(testerCount);
            Console.WriteLine($"ALL {testerCount} TESTERS REGISTERED!");
            await LoadTester.SendMessagesAsync("Gostaria de participar!", messageCount, testerCount);
            var responses = await LoadTester.ReceiveMessagesAsync(TimeSpan.FromSeconds(testerCount), TimeSpan.FromSeconds(1));
            var answer = await ApiAiForStaticContent.GetAnswerAsync(Settings.ConfirmSubscription);
            Assert(responses, answer, messageCount, testerCount);
        }
    }
}