using System.Data.SqlClient;
using MTC2016.Configuration;
using NUnit.Framework;
using Takenet.MessagingHub.Client.Tester;
using MTC2016.ArtificialInteligence;

namespace MTC2016.Tests
{
    public class TestClass<TServiceProvider> : Takenet.MessagingHub.Client.Tester.TestClass<TServiceProvider>
        where TServiceProvider : ApplicationTesterServiceProvider
    {
        protected IApiAiForStaticContent ApiAiForStaticContent { get; private set; }
        protected IApiAiForDynamicContent ApiAiForDynamicContent { get; private set; }

        protected Settings Settings { get; private set; }

        [OneTimeSetUp]
        protected override void SetUp()
        {
            base.SetUp();
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

        [OneTimeTearDown]
        protected override void TearDown()
        {
            base.TearDown();
            ClearTestDataFromDatabase();
        }
    }
}