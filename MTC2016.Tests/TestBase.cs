using System.Threading.Tasks;
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
        protected ApplicationTester<Settings> Tester { get; set; }
        protected IArtificialInteligenceExtension ArtificialInteligenceExtension { get; set; }

        [OneTimeSetUp]
        public void SetUp()
        {
            Tester = new ApplicationTester<Settings>(new ApplicationTesterOptions
            {
                TestServiceProviderType = typeof(TestServiceProvider)
            });
            Tester.StartAsync().Wait();
            ArtificialInteligenceExtension = Tester.GetServiceFromApplicationServiceProvider<IArtificialInteligenceExtension>();
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