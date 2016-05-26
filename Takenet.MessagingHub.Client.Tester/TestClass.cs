using System;
using System.Collections.Generic;
using System.Linq;
using Lime.Protocol;
using Shouldly;

namespace Takenet.MessagingHub.Client.Tester
{
    public class TestClass<TServiceProvider>
        where TServiceProvider : ApplicationTesterServiceProvider
    {
        protected ApplicationLoadTester LoadTester { get; private set; }
        protected ApplicationTester Tester { get; private set; }

        protected virtual void SetUp()
        {
            LoadTester = new ApplicationLoadTester(Options<TServiceProvider>());
            Tester = CreateApplicationTester<TServiceProvider>();
        }

        protected virtual void TearDown()
        {
            Tester.Dispose();
            LoadTester.Dispose();
        }

        protected ApplicationTester CreateApplicationTester<TTestServiceProvider>()
            where TTestServiceProvider : ApplicationTesterServiceProvider
        {
            return new ApplicationTester(Options<TTestServiceProvider>());
        }

        protected virtual ApplicationTesterOptions Options<TTestServiceProvider>()
            where TTestServiceProvider : ApplicationTesterServiceProvider
        {
            return new ApplicationTesterOptions
            {
                TestServiceProviderType = typeof(TTestServiceProvider)
            };
        }

        public static void Assert(Message response, string expected)
        {
            Assert(response, new[] { expected });
        }

        public static void Assert(Message response, IEnumerable<string> expected)
        {
            response.ShouldNotBeNull();
            response.Content.ShouldNotBeNull();
            expected.ShouldContain(response.Content.ToString());
        }

        public static void Assert(string response, string expected)
        {
            Assert(response, new[] { expected });
        }

        public static void Assert(string response, IEnumerable<string> expected)
        {
            response.ShouldNotBeNull();
            expected.ShouldContain(response);
        }

        public static void Assert(IEnumerable<Message> responses, IEnumerable<string> expected, int messageCount, int testerCount)
        {
            int rem;
            Math.DivRem(messageCount, testerCount, out rem);
            if (rem != 0) throw new ArithmeticException($"{messageCount} is not divisible by {testerCount}");

            var share = messageCount / testerCount;

            responses.ShouldNotBeNull();
            responses.GroupBy(m => m.From).Count().ShouldBe(share);
            responses.Count(m => m.Content != null).ShouldBe(messageCount);
            responses.All(m => expected.Contains(m.Content.ToString())).ShouldBeTrue();
        }

        public static void Assert(IEnumerable<Message> responses, string expected, int messageCount, int testerCount)
        {
            Assert(responses, new[] { expected }, messageCount, testerCount);
        }
    }
}