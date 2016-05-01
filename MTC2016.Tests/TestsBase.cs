using MTC2016.Configuration;
using MTC2016.Tests.Mocks;
using NUnit.Framework;
using Takenet.MessagingHub.Client.Tester;
using Lime.Protocol;
using Shouldly;

namespace MTC2016.Tests
{
    public class TestsBase
    {
        protected ApplicationTester<Settings> Tester { get; set; }

        [SetUp]
        public void SetUp()
        {
            Tester = new ApplicationTester<Settings>(new ApplicationTesterOptions
            {
                TestServiceProviderType = typeof(TestServiceProvider)
            });
        }

        [TearDown]
        public void TearDown()
        {
            Tester.Dispose();
        }

        protected static void Assert(Message response, string expected)
        {
            response.ShouldNotBeNull();
            response.Content.ShouldNotBeNull();
            response.Content.ToString().ShouldNotBeNull();
            response.Content.ToString().ShouldBe(expected);
        }

    }
}