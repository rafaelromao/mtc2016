using System.Collections.Generic;
using System.Threading.Tasks;
using Lime.Protocol;
using Takenet.Elephant;

namespace MTC2016
{
    internal interface IDistributionListRecipientsSet
    {
        Task AddAsync(Identity recipient);
        Task<IAsyncEnumerable<Identity>> AsEnumerableAsync();
    }
}