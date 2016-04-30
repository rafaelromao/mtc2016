using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EntroBuilder;
using Lime.Protocol;
using Lime.Protocol.Serialization;
using Newtonsoft.Json;
using Takenet.MessagingHub.Client.Host;
using Takenet.MessagingHub.Client.Listener;

namespace Takenet.MessagingHub.Client.Tester
{
    public class ClientTester : IDisposable
    {
        private static ConsoleTraceListener _listener;
        private static TimeSpan DefaultTimeout => TimeSpan.FromSeconds(1);
        internal static IServiceProvider ApplicationServiceProvider { get; private set; }

        private ConcurrentQueue<Message> _lattestMessages;

        private IMessagingHubClient TestClient { get; set; }
        private IStoppable SmartContact { get; set; }

        private ConcurrentQueue<Message> LattestMessages
        {
            get { return _lattestMessages; }
            set { _lattestMessages = value; }
        }


        public string TesterIdentifier { get; private set; }
        public string TesterAccessKey { get; private set; }
        public Application Application { get; private set; }
        public bool HasCustomTesterIdentifier => TesterIdentifier != Application.Identifier;


        public ClientTester(ClientTesterOptions clientTesterOptions)
        {
            LoadApplicationSettings();

            if (clientTesterOptions.EnableConsoleListener)
                EnableConsoleTraceListener(clientTesterOptions.UseErrorStream);

            PatchApplicationTestSettings(clientTesterOptions);

            DiscardReceivedMessages();
            StartSmartContactAsync().Wait();
            InstantiateTestClient();
            RegisterTestClientMessageReceivers();
            StartTestClientAsync().Wait();
            SleepAsync().Wait();
        }


        private void PatchApplicationTestSettings(ClientTesterOptions clientTesterOptions)
        {
            if (Application.ServiceProviderType != null)
            {
                ValidateApplicationServiceProviderType(Application.ServiceProviderType);
                var applicationServiceProviderType = ParseTypeName(Application.ServiceProviderType);

                ApplicationServiceProvider = ApplicationServiceProvider ?? (IServiceProvider)Activator.CreateInstance(applicationServiceProviderType);
            }

            if (clientTesterOptions.TestServiceProviderType != null)
            {
                ValidateTestServiceProviderType(clientTesterOptions.TestServiceProviderType);
                Application.ServiceProviderType = clientTesterOptions.TestServiceProviderType.Name;
            }

            TesterIdentifier = clientTesterOptions.TesterIdentifier ?? Application.Identifier;
            TesterAccessKey = clientTesterOptions.TesterAccesskey ?? Application.AccessKey;
        }

        private static void ValidateApplicationServiceProviderType(string applicationServiceProviderTypeName)
        {
            var serviceProviderInterfaceType = typeof(IServiceProvider);
            var applicationServiceProviderType = ParseTypeName(applicationServiceProviderTypeName);
            if (!serviceProviderInterfaceType.IsAssignableFrom(applicationServiceProviderType))
                throw new ArgumentException(
                    $"{applicationServiceProviderTypeName} must be a subtype of {serviceProviderInterfaceType.FullName}");
        }

        private static void ValidateTestServiceProviderType(Type testServiceProviderType)
        {
            var baseTestServiceProviderType = typeof (ClientTesterServiceProvider);
            if (!baseTestServiceProviderType.IsAssignableFrom(testServiceProviderType))
                throw new ArgumentException(
                    $"{testServiceProviderType.Name} must be a subtype of {baseTestServiceProviderType.FullName}");
        }

        private static void EnableConsoleTraceListener(bool useErrorStream)
        {
            _listener = new ConsoleTraceListener(useErrorStream);
            Trace.Listeners.Add(_listener);
        }

        [Obsolete("Must use the same method exposed by the TypeUtil")]
        private static Type ParseTypeName(string typeName)
        {
            return TypeUtil
                .GetAllLoadedTypes()
                .FirstOrDefault(t => t.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase)) ??
                       Type.GetType(typeName, true, true);
        }

        private void DiscardReceivedMessages()
        {
            _lattestMessages = new ConcurrentQueue<Message>();
        }

        private async Task StopSmartContactAsync()
        {
            await SmartContact.StopAsync();
        }

        private async Task StartTestClientAsync()
        {
            await TestClient.StartAsync();
        }

        private async Task StopTestClientAsync()
        {
            await TestClient.StopAsync();
        }

        private async Task StartSmartContactAsync()
        {
            SmartContact = await Bootstrapper.StartAsync(Application);
        }

        private void LoadApplicationSettings()
        {
            var assemblyFile = Assembly.GetExecutingAssembly().Location;
            var assemblyDir = new FileInfo(assemblyFile).DirectoryName;
            var appJsonFullName = $"{assemblyDir}\\application.json";
            LoadApplicationJson(appJsonFullName);
            ConfigureWorkingDirectory(appJsonFullName);
        }

        [Obsolete("Bootstrapper should do this automatically")]
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

        private void InstantiateTestClient()
        {
            TestClient = new MessagingHubClientBuilder()
                .UsingEncryption(Application.SessionEncryption.GetValueOrDefault())
                .UsingCompression(Application.SessionCompression.GetValueOrDefault())
                .UsingHostName(Application.HostName ?? "msging.net")
                .UsingDomain(Application.Domain ?? "msging.net")
                .UsingAccessKey(TesterIdentifier, TesterAccessKey)
                .WithSendTimeout(DefaultTimeout)
                .WithMaxConnectionRetries(1)
                .Build();
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
                // Enqueue messages sent to the TestClient
                TestClient.AddMessageReceiver((m, c) => { LattestMessages.Enqueue(m); return Task.CompletedTask; });
            }
            else
            {
                // Ignore messages sent to the SmartContent
                TestClient.AddMessageReceiver(
                    new LambdaMessageReceiver((m, c) => Task.CompletedTask), m => m.MatchReceiverFilters(Application));

                // Enqueue messages sent to the TestClient
                TestClient.AddMessageReceiver(
                    new LambdaMessageReceiver((m, c) => { LattestMessages.Enqueue(m); return Task.CompletedTask; }), m => !m.MatchReceiverFilters(Application));
            }
        }


        public static async Task SleepAsync(int seconds = 1)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
        }

        public virtual void Dispose()
        {
            StopSmartContactAsync().Wait();
            StopTestClientAsync().Wait();
            _listener?.Dispose();
        }

        public virtual void LoadApplicationJson(string appJson)
        {
            var appJsonContent = File.ReadAllText(appJson);
            Application = JsonConvert.DeserializeObject<Application>(appJsonContent);
        }

        public async Task ResetAsync()
        {
            await StopSmartContactAsync();
            await StopTestClientAsync();
            DiscardReceivedMessages();
            await StartSmartContactAsync();
            await StartTestClientAsync();
        }

        public async Task SendMessageAsync(string message)
        {
            await TestClient.SendMessageAsync(message, Application.Identifier);
        }

        public async Task SendMessageAsync<TReceiverType>()
        {
            var receiverName = typeof(TReceiverType).Name;
            var receiverContentFilter = Application.MessageReceivers.SingleOrDefault(r => r.Type == receiverName)?.Content;
            if (string.IsNullOrWhiteSpace(receiverContentFilter))
                throw new ArgumentException($"Could not find a content filter expression for a receiver named {receiverName}!");

            var randomReceiverMessage = GenerateRandomRegexMatch(receiverContentFilter);
            await SendMessageAsync(randomReceiverMessage);
        }

        public async Task<Message> ReceiveMessageAsync(TimeSpan timeout = default(TimeSpan))
        {
            try
            {
                timeout = timeout == default(TimeSpan) ? DefaultTimeout : timeout;
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

        public async Task IgnoreMessageAsync(TimeSpan timeout = default(TimeSpan))
        {
            try
            {
                timeout = timeout == default(TimeSpan) ? DefaultTimeout : timeout;
                using (var cts = new CancellationTokenSource(timeout))
                {
                    while (!cts.IsCancellationRequested)
                    {
                        Message lastMessage;
                        LattestMessages.TryDequeue(out lastMessage);
                        if (lastMessage != null)
                            break;
                        await Task.Delay(10, cts.Token);
                    }
                }
            }
            catch (TaskCanceledException)
            {
            }
        }

        public async Task IgnoreMessagesAsync(TimeSpan timeout = default(TimeSpan))
        {
            try
            {
                timeout = timeout == default(TimeSpan) ? DefaultTimeout : timeout;
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
    }

    public class ClientTester<TSettingsType> : ClientTester
    {
        private void LoadSettings()
        {
            var settingsJsonContent = JsonConvert.SerializeObject(Application.Settings);
            Settings = JsonConvert.DeserializeObject<TSettingsType>(settingsJsonContent);
        }

        public TSettingsType Settings { get; private set; }

        public ClientTester(ClientTesterOptions clientTesterOptions)
            : base(clientTesterOptions)
        {
        }

        public override void LoadApplicationJson(string appJson)
        {
            base.LoadApplicationJson(appJson);
            LoadSettings();
        }
    }
}
