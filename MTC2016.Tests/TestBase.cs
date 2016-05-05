using MTC2016.Configuration;
using MTC2016.Tests.Mocks;
using NUnit.Framework;
using Takenet.MessagingHub.Client.Tester;
using Lime.Protocol;
using MTC2016.ArtificialInteligence;
using Shouldly;

namespace MTC2016.Tests
{
    public class TestBase
    {
        protected ApplicationTester Tester { get; private set; }

        protected IArtificialInteligenceExtension ArtificialInteligenceExtension { get; private set; }

        protected Settings Settings { get; private set; }

        [OneTimeSetUp]
        public void SetUp()
        {
            Tester = new ApplicationTester(new ApplicationTesterOptions
            {
                TestServiceProviderType = typeof(TestServiceProvider)
            });
            Tester.StartAsync().Wait();
            ArtificialInteligenceExtension = Tester.GetServiceFromApplicationServiceProvider<IArtificialInteligenceExtension>();
            Settings = Tester.GetServiceFromApplicationServiceProvider<Settings>();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Tester.StopAsync().Wait();
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