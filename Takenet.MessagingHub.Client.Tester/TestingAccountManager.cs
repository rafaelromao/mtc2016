using System;
using System.Threading;
using System.Threading.Tasks;
using Lime.Messaging.Resources;
using Lime.Protocol;
using Lime.Protocol.Client;
using Lime.Protocol.Network;
using Lime.Protocol.Security;
using Lime.Protocol.Serialization;
using Lime.Transport.Tcp;
using Takenet.MessagingHub.Client.Host;

namespace Takenet.MessagingHub.Client.Tester
{
    internal class TestingAccountManager
    {
        private Application Application { get; }
        private TimeSpan DefaultTimeout { get; }
        private Uri EndPoint { get; }
        private string Domain { get; }

        public TestingAccountManager(Application application, TimeSpan defaultTimeout)
        {
            Application = application;
            DefaultTimeout = defaultTimeout;
            EndPoint = new Uri($"net.tcp://{Application.HostName ?? "msging.net"}:55321");
            Domain = Application.Domain ?? "msging.net";
        }

        public async Task<string> CreateAccountWithAccessKeyAsync(string name, string password)
        {
            await CreateAccountAsync(name, password);
            return await CreateAccessKeyAsync(name, password);
        }

        public async Task CreateAccountAsync(string name, string password)
        {
            IClientChannel clientChannel = null;
            try
            {
                var clientNode = CreateNode(Guid.NewGuid().ToString());
                clientChannel = await EstablishSessionAsync(new GuestAuthentication(), clientNode);

                var setAccountCommandRequest = new Command
                {
                    From = new Node
                    {
                        Name = name,
                        Domain = Domain
                    },
                    Pp = clientChannel.LocalNode,
                    Method = CommandMethod.Set,
                    Uri = new LimeUri(UriTemplates.ACCOUNT),
                    Resource = new Account
                    {
                        Password = password.ToBase64(),
                        InboxSize = 0
                    }
                };

                using (var cts = CreateCancellationTokenSource())
                {
                    var setAccountCommandResponse = await clientChannel.ProcessCommandAsync(setAccountCommandRequest, cts.Token);
                    ThrowIfFailed(setAccountCommandResponse);
                }
            }
            finally
            {
                await EndSession(clientChannel);
            }
        }

        public async Task<string> CreateAccessKeyAsync(string name, string password)
        {
            IClientChannel clientChannel = null;

            try
            {
                using (var authentication = new PlainAuthentication())
                {
                    authentication.SetToBase64Password(password);
                    var clientNode = CreateNode(name);
                    clientChannel = await EstablishSessionAsync(authentication, clientNode);
                }

                using (var cts = CreateCancellationTokenSource())
                {
                    var setAccessKeyCommandResponse = await clientChannel.ProcessCommandAsync(
                        new Command
                        {
                            Resource = new AccountKeyRequest(),
                            Uri = LimeUri.Parse("/account/keys"),
                            Method = CommandMethod.Set
                        },
                        cts.Token);

                    ThrowIfFailed(setAccessKeyCommandResponse);
                }

                using (var cts = CreateCancellationTokenSource())
                {
                    var message = await clientChannel.ReceiveMessageAsync(cts.Token);
                    var accessKey = message.Content as AccessKey;
                    if (accessKey == null)
                    {
                        throw new Exception("Access key was not received");
                    }
                    return accessKey.Key;
                }
            }
            finally
            {
                await EndSession(clientChannel);
            }
        }

        private static void ThrowIfFailed(Command commandResponse)
        {
            if (commandResponse.Status != CommandStatus.Success)
            {
                throw new LimeException(commandResponse.Reason);
            }
        }


        private Node CreateNode(string name) => new Node
        {
            Name = name,
            Domain = Domain,
            Instance = Environment.MachineName.ToLower(),
        };

        private async Task<IClientChannel> EstablishSessionAsync(Authentication authentication, Node clientNode)
        {
            var clientChannel = await CreateAndOpenAsync();
            using (var cts = CreateCancellationTokenSource())
            {
                var session = await clientChannel
                        .EstablishSessionAsync(
                            c => SessionCompression.None,
                            e => SessionEncryption.None,
                            clientNode.ToIdentity(),
                            (s, a) => authentication,
                            Environment.MachineName.ToLower(),
                            cts.Token)
                        .ConfigureAwait(false);

                if (session.State == SessionState.Failed)
                {
                    throw new LimeException(session.Reason);
                }

                return clientChannel;
            }
        }

        private async Task<IClientChannel> CreateAndOpenAsync()
        {
            var transport = new TcpTransport(traceWriter: new TraceWriter());
            var sendTimeout = DefaultTimeout;

            using (var cancellationTokenSource = new CancellationTokenSource(sendTimeout))
            {
                await transport.OpenAsync(EndPoint, cancellationTokenSource.Token);
            }

            var clientChannel = new ClientChannel(
                transport,
                sendTimeout);

            return clientChannel;
        }

        private async Task EndSession(IClientChannel channel)
        {
            if (channel == null) return;
            if (channel.State == SessionState.Established)
            {
                try
                {
                    using (var cts = CreateCancellationTokenSource())
                    {
                        var finisherTask = channel.ReceiveFinishedSessionAsync(cts.Token);
                        await channel.SendFinishingSessionAsync(cts.Token);
                        await finisherTask;
                    }
                }
                catch { }
            }

            channel.DisposeIfDisposable();
        }

        private CancellationTokenSource CreateCancellationTokenSource() => new CancellationTokenSource(DefaultTimeout);
    }
}
