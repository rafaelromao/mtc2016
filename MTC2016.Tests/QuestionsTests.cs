using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MTC2016.Receivers;
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
            Assert(response, Tester.Settings.QuestionWithContent("sobre").Answer);
        }

        [Test]
        public async Task AskObjectiveWithSingleWord()
        {
            var question = Tester.Settings.QuestionWithContent("objetivo");
            var message = Tester.GenerateRandomRegexMatch(question.Content);
            await Tester.SendMessageAsync(message);
            var response = await Tester.ReceiveMessageAsync();
            Assert(response, question.Answer);
        }
    }
}
