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
            const int messageCount = 600;
            const int testerCount = 600;

            Console.WriteLine($"{DateTime.Now} -> START REGISTERING {testerCount} TESTERS!");
            await LoadTester.PrepareTestersAsync(testerCount, Tester.SmartContact);
            Console.WriteLine($"{DateTime.Now} -> ALL {testerCount} TESTERS REGISTERED!");

            Console.WriteLine($"{DateTime.Now} -> START SENDING {messageCount} MESSAGES!");
            await LoadTester.SendMessagesAsync("Gostaria de participar!", messageCount, testerCount, Tester.SmartContact);
            Console.WriteLine($"{DateTime.Now} -> ALL {testerCount} MESSAGES SENT!");

            var responses = await LoadTester.ReceiveMessagesAsync(TimeSpan.FromSeconds(testerCount), TimeSpan.FromSeconds(1));
            var answer = await ApiAiForStaticContent.GetAnswerAsync(Settings.ConfirmSubscription);
            Assert(responses, answer, messageCount, testerCount);
        }
    }
}