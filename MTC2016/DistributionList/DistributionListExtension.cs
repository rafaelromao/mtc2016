using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;

namespace MTC2016.DistributionList
{
    internal class DistributionListExtension : IDistributionListExtension
    {
        private readonly IRecipientsRepository _recipients;

        public DistributionListExtension(IRecipientsRepository recipients)
        {
            _recipients = recipients;
        }

        public async Task<bool> AddAsync(Identity recipient, CancellationToken cancellationToken)
        {
            try
            {
                await _recipients.AddAsync(recipient.ToNode());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<Identity>> GetRecipientsAsync(CancellationToken cancellationToken)
        {
            return await _recipients.AsEnumerableAsync();
        }

        public async Task<bool> ContainsAsync(Identity recipient, CancellationToken cancellationToken)
        {
            return await _recipients.ContainsAsync(recipient.ToNode());
        }

        public async Task<bool> RemoveAsync(Identity recipient, CancellationToken cancellationToken)
        {
            return await _recipients.TryRemoveAsync(recipient.ToNode());
        }
    }
}