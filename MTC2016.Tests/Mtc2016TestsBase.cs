using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTC2016.Configuration;
using Newtonsoft.Json;
using Takenet.MessagingHub.Client;
using Takenet.MessagingHub.Client.Host;

namespace MTC2016.Tests
{
    [TestClass]
    public class Mtc2016TestsBase
    {
        private static CancellationTokenSource NewTimeoutCancellationTokenSource() => new CancellationTokenSource(Timeout);
        private static CancellationToken NewTimeoutCancellationToken() => NewTimeoutCancellationTokenSource().Token;
        private static TimeSpan Timeout => TimeSpan.FromSeconds(20);

        private const string SmartContact = "mtc2016";
        private const string SmartContactTester = "PacienteDoDrBot";
        private const string SmartContactTesterAccessKey = "MXVWeVJD";

        private IMessagingHubClient Client { get; set; }
        private Process SmartContactProcess { get; set; }

        private volatile Message _lastMessage;

        protected Settings Settings { get; private set; }

        [TestInitialize]
        public void TestInitialize()
        {
            SmartContactProcess = StartSmartContact();
            InstantiateSmartContact();
            RegisterMessageReceiver();
            Client.StartAsync(NewTimeoutCancellationToken()).Wait();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Client.StopAsync(NewTimeoutCancellationToken()).Wait();
            SmartContactProcess?.Dispose();
        }

        private Process StartSmartContact()
        {
            var assemblyFile = Assembly.GetExecutingAssembly().Location;
            var assemblyDir = new FileInfo(assemblyFile).DirectoryName;

            var mhh = $"{assemblyDir}\\mhh.exe";
            var appJson = $"{assemblyDir}\\application.json";

            LoadSettings(appJson);

            var process = Process.Start(new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = mhh,
                Arguments = appJson,
                RedirectStandardOutput = true
            });
            Thread.Sleep(TimeSpan.FromSeconds(2));
            return process;
        }

        private void LoadSettings(string appJson)
        {
            var appJsonContent = File.ReadAllText(appJson);
            var application = JsonConvert.DeserializeObject<Application>(appJsonContent);
            var settingsJsonContent = JsonConvert.SerializeObject(application.Settings);
            Settings = JsonConvert.DeserializeObject<Settings>(settingsJsonContent);
        }

        private void InstantiateSmartContact()
        {
            Client = new MessagingHubClientBuilder()
                .UsingEncryption(SessionEncryption.None)
                .UsingAccessKey(SmartContactTester, SmartContactTesterAccessKey)
                .WithSendTimeout(Timeout)
                .Build();
        }

        protected async Task SendMessageAsync(string message)
        {
            await Client.SendMessageAsync(message, SmartContact);
        }

        private void RegisterMessageReceiver()
        {
            Client.AddMessageReceiver((m, c) =>
            {
                _lastMessage = m;
                return Task.CompletedTask;
            });
        }

        protected async Task<string> WaitForResponseAsync()
        {
            using (var cts = NewTimeoutCancellationTokenSource())
            {
                while (!cts.IsCancellationRequested)
                {
                    if (_lastMessage != null)
                        return _lastMessage?.Content.ToString();
                    await Task.Delay(10, cts.Token);
                }
                return null;
            }
        }
    }

    public static class StringExtensions
    {
        public static string ToStringFormatRegex(this string text, int formatPlaceholderCount = 10)
        {
            // Olá, {0}. Não se esqueça da sua consulta.     =>     (\W|^)Olá, [\w ]*[^\W_][\w ]*. Não se esqueça da sua consulta.(\W|$)

            for (var i = 0; i < formatPlaceholderCount; i++)
                text = text.Replace($"{{{i}}}", "[\\w ]*[^\\W_][\\w ]*");

            text = $"(\\W|^){text}(\\W|$)";
            return text;
        }
    }
}
