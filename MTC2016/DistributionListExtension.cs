using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lime.Protocol;

namespace MTC2016
{
    internal class DistributionListExtension : IDistributionListExtension
    {
        private readonly IDistributionListRecipientsList _distributionListRecipientsList;

        public DistributionListExtension(IDistributionListRecipientsList distributionListRecipientsList)
        {
            _distributionListRecipientsList = distributionListRecipientsList;
        }

        public async Task<bool> AddAsync(Identity recipient)
        {
            if (await ContainsAsync(recipient))
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

        public async Task<IEnumerable<Identity>> GetRecipientsAsync()
        {
            return await _distributionListRecipientsList.AsEnumerableAsync();
        }

        public async Task<bool> ContainsAsync(Identity recipient)
        {
            return await _distributionListRecipientsList.ContainsAsync(recipient);
        }

        public async Task<bool> RemoveAsync(Identity recipient)
        {
            return await _distributionListRecipientsList.TryRemoveAsync(recipient);
        }
    }
}