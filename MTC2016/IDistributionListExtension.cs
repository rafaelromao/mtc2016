using System.Collections.Generic;
using System.Threading.Tasks;
using Lime.Protocol;

namespace MTC2016
{
    public interface IDistributionListExtension
    {
        Task<bool> AddAsync(Identity recipient);
        Task<IEnumerable<Identity>> GetRecipientsAsync();
        Task<bool> ContainsAsync(Identity recipient);
        Task<bool> RemoveAsync(Identity recipient);
    }
}