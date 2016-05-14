using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using MTC2016.ArtificialInteligence;
using MTC2016.Configuration;

namespace MTC2016.DistributionList
{
    internal class DistributionListExtension : IDistributionListExtension
    {
        private readonly IUsersRepository _users;

        public DistributionListExtension(IUsersRepository users, Settings settings)
        {
            _users = users;
        }

        public async Task<bool> AddAsync(Identity recipient, CancellationToken cancellationToken)
        {
            if (await ContainsAsync(recipient, cancellationToken))
                return true;

            try
            {
                return await _users.AddUserAsync(recipient.ToNode());
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<Identity>> GetRecipientsAsync(CancellationToken cancellationToken)
        {
            return await _users.GetUsersAsync();
        }

        public async Task<bool> ContainsAsync(Identity recipient, CancellationToken cancellationToken)
        {
            return await _users.ContainsUserAsync(recipient.ToNode());
        }

        public async Task<bool> RemoveAsync(Identity recipient, CancellationToken cancellationToken)
        {
            return await _users.RemoveUserAsync(recipient.ToNode());
        }
    }
}