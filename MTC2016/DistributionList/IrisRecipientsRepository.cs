using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Takenet.Elephant;
using Takenet.MessagingHub.Client.Sender;

namespace MTC2016.DistributionList
{
    internal class IrisRecipientsRepository : IRecipientsRepository
    {
        internal static Node Broadcaster => Node.Parse("postmaster@broadcast.msging.net");
        internal static Identity Mtc2016 => Identity.Parse("mtc2016@broadcast.msging.net");

        private readonly IMessagingHubSender _sender;


        public IrisRecipientsRepository(IMessagingHubSender sender)
        {
            _sender = sender;
        }

        public async Task AddAsync(Identity user)
        {
            var items = new[] { new IdentityDocument { Value = user } };
            await UpdateUsersAsync(items);
        }

        public async Task<bool> ContainsAsync(Identity user)
        {
            var command = new Command(Guid.NewGuid().ToString())
            {
                To = Broadcaster,
                Uri = new LimeUri($"/lists/{Mtc2016}/recipients/{user}"),
                Method = CommandMethod.Get
            };
            var response = await _sender.SendCommandAsync(command);
            return response.Status == CommandStatus.Success;
        }

        public async Task<bool> TryRemoveAsync(Identity user)
        {
            var command = new Command(Guid.NewGuid().ToString())
            {
                To = Broadcaster,
                Method = CommandMethod.Delete,
                Uri = new LimeUri("/lists/{Mtc2016}/recipients/{user}")
            };
            var response = await _sender.SendCommandAsync(command);
            return response.Status == CommandStatus.Success;
        }

        public async Task<IAsyncEnumerable<Identity>> AsEnumerableAsync()
        {
            var command = new Command(Guid.NewGuid().ToString())
            {
                To = Broadcaster,
                Uri = new LimeUri($"/lists/{Mtc2016}/recipients"),
                Method = CommandMethod.Get
            };
            var response = await _sender.SendCommandAsync(command);
            var collection = (DocumentCollection)response.Resource;
            var items = collection.Items;

            throw new NotImplementedException();
            //return items.Select(i => ((IdentityDocument) i).Value);
        }

        internal static Task EnsureMtc2016IsADistributionListAsync(IMessagingHubSender sender, CancellationToken cancellationToken)
        {
            var command = new Command(Guid.NewGuid().ToString())
            {
                To = Broadcaster,
                Method = CommandMethod.Set,
                Uri = new LimeUri($"/lists/{Mtc2016}/recipients"),
                Resource = new IdentityDocument
                {
                    Value = Mtc2016
                }
            };
            return sender.SendCommandAsync(command, cancellationToken);
        }

        private async Task<Command> UpdateUsersAsync(IdentityDocument[] items)
        {
            var command = new Command(Guid.NewGuid().ToString())
            {
                To = Broadcaster,
                Method = CommandMethod.Set,
                Uri = new LimeUri("/lists/{Mtc2016}/recipients"),
                Resource = new DocumentCollection
                {
                    ItemType = IdentityDocument.MediaType,
                    Items = items
                }
            };
            var response = await _sender.SendCommandAsync(command);
            return response;
        }
    }
}