using System.Threading.Tasks;
using MTC2016.Configuration;
using NUnit.Framework;
using Takenet.MessagingHub.Client.Tester;

namespace MTC2016.Tests
{
    [TestFixture]
    public class Mtc2016Tests : MessagingHubClientTester<Settings>
    {
        protected override string TesterIdentifier => Settings.TesterIdentifier;
        protected override string TesterAccessKey => Settings.TesterAccessKey;

        [Test]
        public async Task SubscribeWithSuccess()
        {
            await SendMessageAsync("Entrar");
            await AssertResponseAsync(Settings.Messages.ConfirmSubscription);
        }
    }
}
