using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using EntroBuilder;
using Lime.Protocol;
using Newtonsoft.Json;
using NUnit.Framework;
using Takenet.MessagingHub.Client.Host;
using Takenet.MessagingHub.Client.Listener;

namespace Takenet.MessagingHub.Client.NUnitTester
{
    [TestFixture]
    public abstract class TestClass
    {
        private static CancellationTokenSource NewTimeoutCancellationTokenSource() => new CancellationTokenSource(Timeout);
        private static CancellationToken NewTimeoutCancellationToken() => NewTimeoutCancellationTokenSource().Token;
        private static TimeSpan Timeout => TimeSpan.FromSeconds(20);

        private IMessagingHubClient TestClient { get; set; }
        private IStoppable SmartContact { get; set; }

        private ConcurrentQueue<Message> _lattestMessages;

        private static ConsoleTraceListener _listener;

        protected virtual string TesterIdentifier => Application.Identifier;
        protected virtual string TesterAccessKey => Application.AccessKey;
        protected bool HasCustomTesterIdentifier => TesterIdentifier != Application.Identifier;

        protected Application Application { get; private set; }

        private ConcurrentQueue<Message> LattestMessages
        {
            get { return _lattestMessages; }
            set { _lattestMessages = value; }
        }

        [SetUp]
        public void BaseSetUp()
        {
            IgnoreAllReceivedMessages();
            StartSmartContactAsync().Wait();
            InstantiateTestClient();
            RegisterTestClientMessageReceivers();
            StartTestClientAsync().Wait();
            Delay().Wait();
        }

        protected void IgnoreAllReceivedMessages()
        {
            _lattestMessages = new ConcurrentQueue<Message>();
        }

        protected static async Task Delay(int seconds = 1)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
        }

        [TearDown]
        public void BaseTearDown()
        {
            StopSmartContactAsync().Wait();
            StopTestClientAsync().Wait();
            _listener?.Dispose();
        }

        protected async Task ResetAsync()
        {
            await StopSmartContactAsync();
            await StopTestClientAsync();
            await StartSmartContactAsync();
            await StartTestClientAsync();
        }

        private async Task StopSmartContactAsync()
        {
            await SmartContact.StopAsync();
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

        private async Task StartSmartContactAsync()
        {
            var assemblyFile = Assembly.GetExecutingAssembly().Location;
            var assemblyDir = new FileInfo(assemblyFile).DirectoryName;
            var appJson = $"{assemblyDir}\\application.json";

            LoadApplicationJson(appJson);
            ConfigureWorkingDirectory(appJson); // TODO: Change client and request Bootstrapper to do this by itself

            SmartContact = await Bootstrapper.StartAsync(Application);
        }

        private static void ConfigureWorkingDirectory(string applicationFileName)
        {
            var path = Path.GetDirectoryName(applicationFileName);
            if (!string.IsNullOrWhiteSpace(path))
            {
                Directory.SetCurrentDirectory(path);
            }
            else
            {
                path = Environment.CurrentDirectory;
            }

            AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) =>
            {
                var assemblyName = new AssemblyName(eventArgs.Name);
                var filePath = Path.Combine(path, $"{assemblyName.Name}.dll");
                return File.Exists(filePath) ? Assembly.LoadFile(filePath) : null;
            };
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

        protected async Task SendMessageForReceiverAsync<TReceiverType>()
        {
            var receiverName = typeof(TReceiverType).Name;
            var receiverContentFilter = Application.MessageReceivers.SingleOrDefault(r => r.Type == receiverName)?.Content;
            if (string.IsNullOrWhiteSpace(receiverContentFilter))
                throw new ArgumentException($"Could not find a content filter expression for a receiver named {receiverName}!");

            var randomReceiverMessage = GenerateRandomRegexMatch(receiverContentFilter);
            await SendMessageAsync(randomReceiverMessage);
        }

        private static string GenerateRandomRegexMatch(string pattern)
        {
            try
            {
                var builder = Builder.Create<string>();
                builder = builder.For(Any.ValueLike(pattern));
                return builder.Build();
            }
            catch (Exception)
            {
                throw new ArgumentException($"Invalid regex pattern: {pattern}");
            }
        }

        private void RegisterTestClientMessageReceivers()
        {
            if (HasCustomTesterIdentifier)
            {
                // Ignore messages sent to the SmartContent
                TestClient.AddMessageReceiver(
                    new LambdaMessageReceiver((m, c) => Task.CompletedTask), m => m.MatchReceiverFilters(Application));

                // Enqueue messages sent to the TestClient
                TestClient.AddMessageReceiver(
                    new LambdaMessageReceiver((m, c) => { LattestMessages.Enqueue(m); return Task.CompletedTask; }), m => !m.MatchReceiverFilters(Application));
            }
            else
            {
                // Enqueue messages sent to the TestClient
                TestClient.AddMessageReceiver((m, c) => { LattestMessages.Enqueue(m); return Task.CompletedTask; });
            }
        }

        protected async Task<Message> DequeueReceivedMessageAsync(TimeSpan timeout = default(TimeSpan))
        {
            try
            {
                timeout = timeout == default(TimeSpan) ? Timeout : timeout;
                using (var cts = new CancellationTokenSource(timeout))
                {
                    while (!cts.IsCancellationRequested)
                    {
                        Message lastMessage;
                        if (LattestMessages.TryDequeue(out lastMessage))
                            return lastMessage;
                        await Task.Delay(10, cts.Token);
                    }
                    return null;
                }
            }
            catch (TaskCanceledException)
            {
                return null;
            }
        }

        protected async Task IgnoreReceivedMessageAsync(TimeSpan timeout = default(TimeSpan))
        {
            try { 
            timeout = timeout == default(TimeSpan) ? Timeout : timeout;
            using (var cts = new CancellationTokenSource(timeout))
            {
                while (!cts.IsCancellationRequested)
                {
                    Message lastMessage;
                    LattestMessages.TryDequeue(out lastMessage);
                    await Task.Delay(10, cts.Token);
                }
            }
            }
            catch (TaskCanceledException)
            {
            }
        }

        protected async Task AssertLastReceivedMessageAsync(string expected, bool isRegex = false, int formatPlaceholderCount = 10)
        {
            var message = await DequeueReceivedMessageAsync();
            var content = message?.Content?.ToString();
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

    public static class MessageExtensions
    {
        public static bool MatchReceiverFilters(this Message message, Application application)
        {
            var filters = application.MessageReceivers.Select(r => r.Content);
            var content = message?.Content?.ToString();
            var result = !string.IsNullOrWhiteSpace(content) && filters.Any(f => Regex.IsMatch(content, f, RegexOptions.Compiled | RegexOptions.IgnoreCase));
            return result;
        }
    }

}
