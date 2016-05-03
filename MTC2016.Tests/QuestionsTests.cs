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
        public async Task AskAboutWithSingleWord()
        {
            await Tester.SendMessageAsync("sobre");
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ArtificialInteligenceExtension.GetAnswerAsync("sobre");
            Assert(response, answer);
        }

        [Test]
        public async Task AskObjectiveWithSingleWord()
        {
            await Tester.SendMessageAsync("objetivo");
            var response = await Tester.ReceiveMessageAsync();
            var answer = await ArtificialInteligenceExtension.GetAnswerAsync("objetivo");
            Assert(response, answer);
        }
    }
}
