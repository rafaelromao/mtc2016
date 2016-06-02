using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using MTC2016.Receivers;
using MTC2016.Tests.Mocks;
using NUnit.Framework;
using Shouldly;

namespace MTC2016.Tests
{
    [TestFixture]
    public class QuestionsTests : TestClass<QuestionMessageReceiverThatDoesNotParseImagesServiceProvider>
    {
        [Test]
        [TestCase("Fale me sobre o evento!")]
        [TestCase("Sobre o evento")]
        [TestCase("Sobre o que é este evento?")]
        public async Task AskAbout(string intent)
        {
            await Tester.SendMessageAsync(intent);
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ApiAiForStaticContent.GetAnswerAsync(intent);
            Assert(response, answer);
        }

        [Test]
        [TestCase("Quais os objetivos do evento?")]
        public async Task AskObjective(string intent)
        {
            await Tester.SendMessageAsync(intent);
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ApiAiForStaticContent.GetAnswerAsync(intent);
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
            var answer = await ApiAiForStaticContent.GetAnswerAsync(intent);
            Assert(response, answer);
        }


        [Test]
        [TestCase("MobileApps?")]
        [TestCase("Palestra sobre MobileApps?")]
        public async Task AskAboutTalk(string intent)
        {
            await Tester.SendMessageAsync(intent);
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ApiAiForStaticContent.GetAnswerAsync(intent);
            Assert(response, answer);
        }

        [Test]
        public async Task AskMenuTalk()
        {
            var intent = "Menu";
            await Tester.SendMessageAsync(intent);
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ApiAiForStaticContent.GetAnswerAsync(intent);
            answer = QuestionMessageReceiver.FixOMNISelectMessageForOtherDomains(answer);
            Assert(response, answer);
        }

        [Test]
        [TestCase("#")]
        [TestCase("#asdasda")]
        [TestCase("#asdasd#")]
        [TestCase("asdasdasd#")]
        [TestCase("asdasdasd#asdasdasd")]
        [TestCase("%")]
        [TestCase("?")]
        [TestCase("$")]
        [TestCase("¨")]
        [TestCase("(")]
        [TestCase(")")]
        [TestCase("{")]
        [TestCase("}")]
        [TestCase("[")]
        [TestCase("]")]
        [TestCase("+")]
        [TestCase("\\")]
        [TestCase("/")]
        [TestCase("\"")]
        [TestCase("")]
        [TestCase(" ")]
        public async Task AskStrangeCharactersTalk(string query)
        {
            await Tester.SendMessageAsync(query);
            var response = await Tester.ReceiveMessageAsync();
            var intent = await ApiAiForStaticContent.GetIntentAsync(Settings.CouldNotUnderstand);
            var speechs = intent.Responses.SelectMany(r => r.Speech);
            Assert(response, speechs);
        }
    }
}
