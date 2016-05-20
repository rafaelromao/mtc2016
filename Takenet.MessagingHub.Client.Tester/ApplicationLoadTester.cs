using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lime.Messaging.Contents;
using Lime.Protocol;

namespace Takenet.MessagingHub.Client.Tester
{
    public sealed class ApplicationLoadTester : IDisposable
    {
        private readonly Dictionary<int, ApplicationTester> _testers = new Dictionary<int, ApplicationTester>();
        private readonly ApplicationTesterOptions _options;

        public ApplicationLoadTester(ApplicationTesterOptions options)
        {
            _options = options;
        }

        public async Task SendMessageAsync(Document message, int messageCount, int testerCount)
        {
            int rem;
            Math.DivRem(messageCount, testerCount, out rem);
            if (rem != 0) throw new ArithmeticException($"{messageCount} is not divisible by {testerCount}");

            var share = messageCount / testerCount;
            for (var testerCounter = 0; testerCounter < testerCount; testerCounter++)
            {
                var tester = await GetTesterAsync(testerCounter);
                for (var messageCounter = 0; messageCounter < share; messageCounter++)
                {
                    await tester.SendMessageAsync(message);
                }
            }
        }

        public Task SendMessageAsync(string message, int messageCount, int testerCount)
        {
            return SendMessageAsync(new PlainText { Text = message}, messageCount, testerCount);
        }

        public Task<Message> ReceiveMessageAsync(int testerIndex, TimeSpan timeout)
        {
            return _testers[testerIndex].ReceiveMessageAsync(timeout);
        }

        public Task IgnoreMessageAsync(int testerIndex, TimeSpan timeout)
        {
            return _testers[testerIndex].IgnoreMessageAsync(timeout);
        }

        private async Task<ApplicationTester> GetTesterAsync(int testerIndex)
        {
            if (_testers.ContainsKey(testerIndex))
                return _testers[testerIndex];

            var options = _options.Clone();
            options.TesterAccountIndex = testerIndex;
            var result = new ApplicationTester(_options);
            _testers[testerIndex] = result;
            return result;
        }

        public void Dispose()
        {
            _testers.Values.ForEach(t =>
            {
                t.Dispose();
            });
        }
    }
}
