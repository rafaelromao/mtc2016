using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lime.Protocol;

namespace MTC2016
{
    internal class DistributionListExtension : IDistributionListExtension
    {
        private readonly IDistributionListRecipientsSet _distributionListRecipientsSet;

        public DistributionListExtension(IDistributionListRecipientsSet distributionListRecipientsSet)
        {
            _distributionListRecipientsSet = distributionListRecipientsSet;
        }

        public async Task<bool> AddAsync(Identity recipient)
        {
            try
            {
                await _distributionListRecipientsSet.AddAsync(recipient);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<Identity>> GetAllAsync()
        {
            return await _distributionListRecipientsSet.AsEnumerableAsync();
        }
    }
}