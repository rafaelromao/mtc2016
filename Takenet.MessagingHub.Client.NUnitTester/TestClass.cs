using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Newtonsoft.Json;
using NUnit.Framework;
using Takenet.MessagingHub.Client.Host;

namespace Takenet.MessagingHub.Client.NUnitTester
{
    [TestFixture]
    public abstract class TestClass
    {
        private static CancellationTokenSource NewTimeoutCancellationTokenSource() => new CancellationTokenSource(Timeout);
        private static CancellationToken NewTimeoutCancellationToken() => NewTimeoutCancellationTokenSource().Token;
        private static TimeSpan Timeout => TimeSpan.FromSeconds(20);

        private IMessagingHubClient TestClient { get; set; }
        private Process SmartContactProcess { get; set; }

        private readonly ConcurrentQueue<Message> _lattestMessages = new ConcurrentQueue<Message>();
        private static ConsoleTraceListener _listener;

        protected abstract string TesterIdentifier { get; }
        protected abstract string TesterAccessKey { get; }

        protected Application Application { get; private set; }

        [OneTimeSetUp]
        public virtual async Task OneTimeSetUp()
        {
            SmartContactProcess = StartSmartContact();
            InstantiateTestClient();
            RegisterMessageReceiver();
            await StartTestClientAsync();
        }

        [OneTimeTearDown]
        public virtual async Task OneTimeTearDown()
        {
            SmartContactProcess?.Dispose();
            await StopTestClientAsync();
            _listener?.Dispose();
        }

        private async Task StartTestClientAsync()
        {
            var cancellationToken = NewTimeoutCancellationToken();
            await TestClient.StartAsync(cancellationToken);
        }

        private async Task StopTestClientAsync()
        {
            await TestClient.StopAsync(NewTimeoutCancellationToken());
        }

        protected void EnableConsoleTraceListener(bool useErrorStream = false)
        {
            _listener = new ConsoleTraceListener(useErrorStream);
            Trace.Listeners.Add(_listener);
        }

        private Process StartSmartContact()
        {
            var assemblyFile = Assembly.GetExecutingAssembly().Location;
            var assemblyDir = new FileInfo(assemblyFile).DirectoryName;

            var mhh = $"{assemblyDir}\\mhh.exe";
            var appJson = $"{assemblyDir}\\application.json";

            LoadApplicationJson(appJson);

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

        protected virtual void LoadApplicationJson(string appJson)
        {
            var appJsonContent = File.ReadAllText(appJson);
            Application = JsonConvert.DeserializeObject<Application>(appJsonContent);
        }


        private void InstantiateTestClient()
        {
            TestClient = new MessagingHubClientBuilder()
                .UsingEncryption(Application.SessionEncryption.GetValueOrDefault())
                .UsingCompression(Application.SessionCompression.GetValueOrDefault())
                .UsingHostName(Application.HostName ?? "msging.net")
                .UsingDomain(Application.Domain ?? "msging.net")
                .UsingAccessKey(TesterIdentifier, TesterAccessKey)
                .WithSendTimeout(Timeout)
                .WithMaxConnectionRetries(1)
                .Build();
        }

        protected async Task SendMessageAsync(string message)
        {
            await TestClient.SendMessageAsync(message, Application.Identifier);
        }

        private void RegisterMessageReceiver()
        {
            TestClient.AddMessageReceiver((m, c) =>
            {
                _lattestMessages.Enqueue(m);
                return Task.CompletedTask;
            });
        }

        protected async Task<Message> DequeueResponseAsync()
        {
            using (var cts = NewTimeoutCancellationTokenSource())
            {
                while (!cts.IsCancellationRequested)
                {
                    Message lastMessage;
                    if (_lattestMessages.TryDequeue(out lastMessage))
                        return lastMessage;
                    await Task.Delay(10, cts.Token);
                }
                return null;
            }
        }

        protected async Task AssertResponseAsync(string expected, bool isRegex = false, int formatPlaceholderCount = 10)
        {
            var response = await DequeueResponseAsync();
            var content = response.Content.ToString();
            if (isRegex)
            {
                Assert.True(Regex.IsMatch(content, expected.ToStringFormatRegex(formatPlaceholderCount)));
            }
            else
            {
                Assert.AreEqual(expected, content);
            }
        }
    }

    public abstract class TestClass<TSettingsType> : TestClass
    {
        private void LoadSettings()
        {
            var settingsJsonContent = JsonConvert.SerializeObject(Application.Settings);
            Settings = JsonConvert.DeserializeObject<TSettingsType>(settingsJsonContent);
        }

        protected TSettingsType Settings { get; private set; }

        protected override void LoadApplicationJson(string appJson)
        {
            base.LoadApplicationJson(appJson);
            LoadSettings();
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
