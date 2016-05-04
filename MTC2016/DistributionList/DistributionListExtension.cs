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
        private readonly IArtificialInteligenceExtension _artificialInteligenceExtension;
        private readonly Settings _settings;

        public DistributionListExtension(IArtificialInteligenceExtension artificialInteligenceExtension, Settings settings)
        {
            _artificialInteligenceExtension = artificialInteligenceExtension;
            _settings = settings;
        }

        public async Task<bool> AddAsync(Identity recipient, CancellationToken cancellationToken)
        {
            if (await ContainsAsync(recipient, cancellationToken))
                return true;

            try
            {
                await _artificialInteligenceExtension.AddEntityAsync(recipient.ToString());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<Identity>> GetRecipientsAsync(CancellationToken cancellationToken)
        {
            return await _artificialInteligenceExtension.GetUsersAsync();
        }

        public async Task<bool> ContainsAsync(Identity recipient, CancellationToken cancellationToken)
        {
            return await _artificialInteligenceExtension.ContainsUserAsync(recipient);
        }

        public async Task<bool> RemoveAsync(Identity recipient, CancellationToken cancellationToken)
        {
            return await _artificialInteligenceExtension.RemoveUserAsync(recipient);
        }
    }
}