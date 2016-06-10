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

            Console.WriteLine($"{DateTime.Now} -> START REGISTERING {testerCount} TESTERS!");
            await LoadTester.PrepareTestersAsync(testerCount);
            Console.WriteLine($"{DateTime.Now} -> ALL {testerCount} TESTERS REGISTERED!");

            Console.WriteLine($"{DateTime.Now} -> START SENDING {messageCount} MESSAGES!");
            await LoadTester.SendMessagesAsync("Gostaria de participar!", messageCount, testerCount);
            Console.WriteLine($"{DateTime.Now} -> ALL {testerCount} MESSAGES SENT!");

            var responses = await LoadTester.ReceiveMessagesAsync(TimeSpan.FromSeconds(testerCount * 3), TimeSpan.FromSeconds(3));
            var answer = await ApiAiForStaticContent.GetAnswerAsync(Settings.ConfirmSubscription);
            Assert(responses, answer, messageCount, testerCount);
        }
    }
}