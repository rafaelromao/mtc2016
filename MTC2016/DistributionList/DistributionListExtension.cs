using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;

namespace MTC2016.DistributionList
{
    internal class DistributionListExtension : IDistributionListExtension
    {
        private readonly IDistributionListRecipientsList _distributionListRecipientsList;

        public DistributionListExtension(IDistributionListRecipientsList distributionListRecipientsList)
        {
            _distributionListRecipientsList = distributionListRecipientsList;
        }

        public async Task<bool> AddAsync(Identity recipient, CancellationToken cancellationToken)
        {
            if (await ContainsAsync(recipient, cancellationToken))
                return true;

            try
            {
                await _distributionListRecipientsList.AddAsync(recipient);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<Identity>> GetRecipientsAsync(CancellationToken cancellationToken)
        {
            return await _distributionListRecipientsList.AsEnumerableAsync();
        }

        public async Task<bool> ContainsAsync(Identity recipient, CancellationToken cancellationToken)
        {
            return await _distributionListRecipientsList.ContainsAsync(recipient);
        }

        public async Task<bool> RemoveAsync(Identity recipient, CancellationToken cancellationToken)
        {
            return await _distributionListRecipientsList.TryRemoveAsync(recipient);
        }
    }
}