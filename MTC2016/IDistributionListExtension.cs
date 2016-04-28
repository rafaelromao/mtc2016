using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;

namespace MTC2016
{
    public interface IDistributionListExtension
    {
        Task<bool> AddAsync(Identity recipient, CancellationToken cancellationToken);
        Task<IEnumerable<Identity>> GetRecipientsAsync(CancellationToken cancellationToken);
        Task<bool> ContainsAsync(Identity recipient, CancellationToken cancellationToken);
        Task<bool> RemoveAsync(Identity recipient, CancellationToken cancellationToken);
    }
}