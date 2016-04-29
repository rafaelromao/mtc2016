using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Takenet.MessagingHub.Client;

namespace MTC2016.Tests
{
    [TestClass]
    public class Mtc2016Tests : Mtc2016TestsBase
    {
        [TestMethod]
        public async Task SubscribeWithSuccess()
        {
            await SendMessageAsync("Entrar");
            var response = await WaitForResponseAsync();
            response.ShouldBe(Settings.Messages.ConfirmSubscription);
        }
    }
}
