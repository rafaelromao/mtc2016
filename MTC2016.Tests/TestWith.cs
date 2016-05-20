using System.Data.SqlClient;
using MTC2016.Configuration;
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

        protected IApiAiForStaticContent ApiAiForStaticContent { get; private set; }
        protected IApiAiForDynamicContent ApiAiForDynamicContent { get; private set; }

        protected Settings Settings { get; private set; }

        [OneTimeSetUp]
        public void SetUp()
        {
            Tester = CreateApplicationTester<TServiceProvider>();
            ApiAiForStaticContent = Tester.GetService<IApiAiForStaticContent>();
            ApiAiForDynamicContent = Tester.GetService<IApiAiForDynamicContent>();
            Settings = Tester.GetService<Settings>();
            ClearTestDataFromDatabase();
        }

        private void ClearTestDataFromDatabase()
        {
            using (var connection = new SqlConnection(Settings.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("DELETE FROM [FEEDBACK] WHERE [FROM] LIKE '%$tester%'", connection))
                {
                    command.ExecuteNonQuery();
                }
                using (var command = new SqlCommand("DELETE FROM [IDENTITY] WHERE [NAME] LIKE '%$tester%'", connection))
                {
                    command.ExecuteNonQuery();
                }
            }
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
            ClearTestDataFromDatabase();
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