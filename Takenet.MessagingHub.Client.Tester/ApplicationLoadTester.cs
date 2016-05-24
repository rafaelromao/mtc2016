using System;
using System.Collections.Generic;
using System.Threading;
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

        public async Task SendMessagesAsync(Document message, int messageCount, int testerCount)
        {
            int rem;
            Math.DivRem(messageCount, testerCount, out rem);
            if (rem != 0) throw new ArithmeticException($"{messageCount} is not divisible by {testerCount}");

            var share = messageCount / testerCount;
            for (var testerCounter = 0; testerCounter < testerCount; testerCounter++)
            {
                var tester = GetTester(testerCounter);
                for (var messageCounter = 0; messageCounter < share; messageCounter++)
                {
                    await tester.SendMessageAsync(message);
                }
            }
        }

        public Task SendMessagesAsync(string message, int messageCount, int testerCount)
        {
            return SendMessagesAsync(new PlainText { Text = message}, messageCount, testerCount);
        }

        public Task<Message> ReceiveMessageAsync(int testerIndex, TimeSpan timeout)
        {
            return _testers[testerIndex].ReceiveMessageAsync(timeout);
        }

        public async Task<IEnumerable<Message>> ReceiveMessagesAsync(int testerIndex, TimeSpan timeout)
        {
            var result = new List<Message>();
            var cts = new CancellationTokenSource(timeout);
            while (!cts.IsCancellationRequested)
            {
                var message = await _testers[testerIndex].ReceiveMessageAsync(timeout);
                result.Add(message);
            }
            return result;
        }

        public async Task<IEnumerable<Message>> ReceiveMessagesAsync(TimeSpan timeout)
        {
            var result = new List<Message>();
            var cts = new CancellationTokenSource(timeout);
            while (!cts.IsCancellationRequested)
            {
                foreach (var tester in _testers.Values)
                {
                    var message = await tester.ReceiveMessageAsync(timeout);
                    result.Add(message);
                }
            }
            return result;
        }

        public Task IgnoreMessageAsync(int testerIndex, TimeSpan timeout)
        {
            return _testers[testerIndex].IgnoreMessageAsync(timeout);
        }

        private ApplicationTester GetTester(int testerIndex)
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
