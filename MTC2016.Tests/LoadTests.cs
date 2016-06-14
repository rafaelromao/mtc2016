using System;
using System.Threading.Tasks;
using MTC2016.Tests.Mocks;
using NUnit.Framework;

namespace MTC2016.Tests
{
    [TestFixture]
    public class LoadTests : TestClass<NoSubscriptionAndSingleFakeScheduleServiceProvider>
    {
        [Test][Ignore("")]
        public async Task FiveHundredSubscriptions()
        {
            const int messageCount = 60;
            const int testerCount = 60;

            Console.WriteLine($"{DateTime.Now} -> START REGISTERING {testerCount} TESTERS!");
            await LoadTester.PrepareTestersAsync(testerCount);
            Console.WriteLine($"{DateTime.Now} -> ALL {testerCount} TESTERS REGISTERED!");

            Console.WriteLine($"{DateTime.Now} -> START SENDING {messageCount} MESSAGES!");
            await LoadTester.SendMessagesAsync("Gostaria de participar!", messageCount, testerCount);
            Console.WriteLine($"{DateTime.Now} -> ALL {testerCount} MESSAGES SENT!");

            var responses = await LoadTester.ReceiveMessagesAsync(TimeSpan.FromSeconds(testerCount * 10), TimeSpan.FromSeconds(8));
            var answer = await ApiAiForStaticContent.GetAnswerAsync(Settings.ConfirmSubscription);
            Assert(responses, answer, messageCount, testerCount);
        }
    }
}