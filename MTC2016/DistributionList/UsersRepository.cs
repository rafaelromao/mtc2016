using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Takenet.MessagingHub.Client.Sender;

namespace MTC2016.DistributionList
{
    internal class UsersRepository : IUsersRepository
    {
        internal static Node Broadcaster => Node.Parse("postmaster@broadcast.msging.net");
        internal static Identity Mtc2016 => Identity.Parse("mtc2016@broadcast.msging.net");

        private readonly IMessagingHubSender _sender;


        public UsersRepository(IMessagingHubSender sender)
        {
            _sender = sender;
        }

        public async Task<bool> AddUserAsync(Identity user)
        {
            var users = await GetUsersAsync();
            users = new List<Identity>(users);
            ((List<Identity>)users).Add(user);

            var items = users.Select(u => new IdentityDocument {Value = u}).ToArray();
            var response = await UpdateUsersAsync(items);
            return response.Status == CommandStatus.Success;
        }

        public async Task<IEnumerable<Identity>> GetUsersAsync()
        {
            var command = new Command(Guid.NewGuid())
            {
                To = Broadcaster,
                Uri = new LimeUri($"/lists/{Mtc2016}"),
                Method = CommandMethod.Get
            };
            var response = await _sender.SendCommandAsync(command);
            var collection = (DocumentCollection)response.Resource;
            var items = collection.Items;
            return items.Select(i => ((IdentityDocument)i).Value);
        }

        public async Task<bool> ContainsUserAsync(Identity user)
        {
            var users = await GetUsersAsync();
            return users.Any(u => u.Equals(user));
        }

        public async Task<bool> RemoveUserAsync(Identity user)
        {
            var users = await GetUsersAsync();
            users = new List<Identity>(users);
            user = users.FirstOrDefault(u => u.Equals(user));

            if (user == null) return false;

            ((List<Identity>) users).Remove(user);
            return true;
        }

        internal static Task EnsureMtc2016IsADistributionListAsync(IMessagingHubSender sender, CancellationToken cancellationToken)
        {
            var command = new Command(Guid.NewGuid())
            {
                To = Broadcaster,
                Method = CommandMethod.Set,
                Uri = new LimeUri($"/lists/{Mtc2016}"),
                Resource = new IdentityDocument
                {
                    Value = Mtc2016
                }
            };
            return sender.SendCommandAsync(command, cancellationToken);
         }

        private async Task<Command> UpdateUsersAsync(IdentityDocument[] items)
        {
            var command = new Command(Guid.NewGuid())
            {
                To = Broadcaster,
                Method = CommandMethod.Set,
                Uri = new LimeUri("/lists"),
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