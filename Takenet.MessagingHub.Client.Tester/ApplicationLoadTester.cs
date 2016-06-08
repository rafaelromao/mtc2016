﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lime.Messaging.Contents;
using Lime.Protocol;
using Takenet.MessagingHub.Client.Listener;

namespace Takenet.MessagingHub.Client.Tester
{
    public sealed class ApplicationLoadTester : IDisposable
    {
        private readonly IDictionary<int, ApplicationTester> _testers = new ConcurrentDictionary<int, ApplicationTester>();
        private readonly ApplicationTesterOptions _options;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public ApplicationLoadTester(ApplicationTesterOptions options)
        {
            _options = options;
        }


        private Task<ApplicationTester> GetTesterAsync(int testerIndex, IStoppable smartContact)
        {
            return Task.Run(() =>
            {
                if (_testers.ContainsKey(testerIndex))
                    return _testers[testerIndex];

                var options = _options.Clone();
                options.TesterAccountIndex = testerIndex + 1;
                var result = new ApplicationTester(options, smartContact);
                _testers[testerIndex] = result;
                return result;
            });
        }

        public void Dispose()
        {
            _testers.Values.ForEach(t =>
            {
                t.Dispose();
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="testerCount"></param>
        /// <param name="smartContact"></param>
        /// <returns></returns>
        public async Task PrepareTestersAsync(int testerCount, IStoppable smartContact = null)
        {
            foreach (var testerCounter in Enumerable.Range(0, testerCount).AsParallel())
            {
                await GetTesterAsync(testerCounter, smartContact);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageCount"></param>
        /// <param name="testerCount"></param>
        /// <param name="smartContact"></param>
        /// <returns></returns>
        public async Task SendMessagesAsync(Document message, int messageCount, int testerCount, IStoppable smartContact = null)
        {
            int rem;
            Math.DivRem(messageCount, testerCount, out rem);
            if (rem != 0) throw new ArithmeticException($"{messageCount} is not divisible by {testerCount}");

            var share = messageCount / testerCount;
            foreach (var testerCounter in Enumerable.Range(0, testerCount).AsParallel())
            {
                var tester = await GetTesterAsync(testerCounter, smartContact);
                for (var messageCounter = 0; messageCounter < share; messageCounter++)
                {
                    await tester.SendMessageAsync(message);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageCount"></param>
        /// <param name="testerCount"></param>
        /// <param name="smartContact"></param>
        /// <returns></returns>
        public Task SendMessagesAsync(string message, int messageCount, int testerCount, IStoppable smartContact = null)
        {
            return SendMessagesAsync(new PlainText { Text = message }, messageCount, testerCount, smartContact);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="testerIndex"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task<Message> ReceiveMessageAsync(int testerIndex, TimeSpan timeout)
        {
            return _testers[testerIndex].ReceiveMessageAsync(timeout);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="testerIndex"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allMessagesTimeout"></param>
        /// <param name="singleMessagesTimeout"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Message>> ReceiveMessagesAsync(TimeSpan allMessagesTimeout, TimeSpan singleMessagesTimeout)
        {
            var result = new List<Message>();
            var cts = new CancellationTokenSource(allMessagesTimeout);
            while (!cts.IsCancellationRequested)
            {
                foreach (var tester in _testers.Values)
                {
                    var message = await tester.ReceiveMessageAsync(singleMessagesTimeout);
                    if (message != null)
                    {
                        result.Add(message);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="testerIndex"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task IgnoreMessageAsync(int testerIndex, TimeSpan timeout)
        {
            return _testers[testerIndex].IgnoreMessageAsync(timeout);
        }
    }
}
