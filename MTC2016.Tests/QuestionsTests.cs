using System.Threading.Tasks;
using MTC2016.Tests.Mocks;
using NUnit.Framework;

namespace MTC2016.Tests
{
    [TestFixture]
    public class QuestionsTests : TestBase<TestsServiceProvider>
    {
        [Test]
        [TestCase("Fale me sobre o evento!")]
        [TestCase("Sobre o evento")]
        [TestCase("Sobre o que é este evento?")]
        public async Task AskAbout(string intent)
        {
            await Tester.SendMessageAsync(intent);
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ArtificialInteligenceExtension.GetAnswerAsync(intent);
            Assert(response, answer);
        }

        [Test]
        [TestCase("Quais os objetivos do evento?")]
        public async Task AskObjective(string intent)
        {
            await Tester.SendMessageAsync(intent);
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ArtificialInteligenceExtension.GetAnswerAsync(intent);
            Assert(response, answer);
        }

        [Test]
        [TestCase("Sthanley?")]
        [TestCase("Quem é o Sthanley?")]
        [TestCase("Palestrante Sthanley?")]
        [TestCase("Sobre o Sthanley?")]
        [TestCase("Sobre o palestrante Sthanley?")]
        [TestCase("Letícia?")]
        [TestCase("Quem é a Letícia?")]
        [TestCase("Palestrante Letícia?")]
        [TestCase("Sobre a Letícia?")]
        [TestCase("Sobre a palestrante Letícia?")]
        public async Task AskAboutSpeaker(string intent)
        {
            await Tester.SendMessageAsync(intent);
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ArtificialInteligenceExtension.GetAnswerAsync(intent);
            Assert(response, answer);
        }


        [Test]
        [TestCase("MobileApps?")]
        [TestCase("Palestra MobileApps?")]
        public async Task AskAboutTalk(string intent)
        {
            await Tester.SendMessageAsync(intent);
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ArtificialInteligenceExtension.GetAnswerAsync(intent);
            Assert(response, answer);
        }
    }
}
