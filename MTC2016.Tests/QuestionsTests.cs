using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MTC2016.Receivers;
using MTC2016.Tests.Mocks;
using NUnit.Framework;

namespace MTC2016.Tests
{
    [TestFixture]
    public class QuestionsTests : TestBase
    {
        [Test]
        public async Task AskAbout()
        {
            var intent = "Fale me sobre o evento!";
            await Tester.SendMessageAsync(intent);
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ArtificialInteligenceExtension.GetAnswerAsync(intent);
            Assert(response, answer);
        }

        [Test]
        public async Task AskObjective()
        {
            var intent = "Quais os objetivos do evento?";
            await Tester.SendMessageAsync(intent);
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ArtificialInteligenceExtension.GetAnswerAsync(intent);
            Assert(response, answer);
        }
    }
}
