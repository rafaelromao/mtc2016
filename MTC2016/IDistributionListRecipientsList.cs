using System.Collections.Generic;
using System.Threading.Tasks;
using Lime.Protocol;
using Takenet.Elephant;

namespace MTC2016
{
    internal interface IDistributionListRecipientsList
    {
        Task AddAsync(Identity recipient);
        Task<IAsyncEnumerable<Identity>> AsEnumerableAsync();
        Task<bool> ContainsAsync(Identity recipient);
        Task<bool> TryRemoveAsync(Identity recipient);
    }
}