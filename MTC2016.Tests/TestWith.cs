using System;
using System.Linq;
using MTC2016.Configuration;
using MTC2016.Tests.Mocks;
using NUnit.Framework;
using Takenet.MessagingHub.Client.Tester;
using Lime.Protocol;
using MTC2016.ArtificialInteligence;
using Shouldly;

namespace MTC2016.Tests
{
    public class TestWith<TServiceProvider>
        where TServiceProvider : ApplicationTesterServiceProvider
    {
        protected ApplicationTester Tester { get; private set; }

        protected IArtificialInteligenceExtension ArtificialInteligenceExtension { get; private set; }

        protected Settings Settings { get; private set; }

        [OneTimeSetUp]
        public void SetUp()
        {
            Tester = CreateApplicationTester<TServiceProvider>();
            Tester.StartAsync().Wait();
            ArtificialInteligenceExtension = Tester.GetService<IArtificialInteligenceExtension>();
            Settings = Tester.GetService<Settings>();
        }

        protected ApplicationTester CreateApplicationTester<TTestServiceProvider>()
            where TTestServiceProvider : ApplicationTesterServiceProvider
        {
            return new ApplicationTester(new ApplicationTesterOptions
            {
                TestServiceProviderType = typeof(TTestServiceProvider)
            });
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Tester.StopAsync().Wait();
            Tester.Dispose();
        }

        public static void Assert(Message response, string expected)
        {
            response.ShouldNotBeNull();
            response.Content.ShouldNotBeNull();
            response.Content.ToString().ShouldNotBeNull();
            response.Content.ToString().ShouldBe(expected);
        }

        public static void Assert(string actual, string expected)
        {
            actual.ShouldNotBeNull();
            actual.ShouldBe(expected);
        }
    }
}