using System.Threading.Tasks;
using Lime.Protocol;
using Takenet.Elephant;

namespace MTC2016.DistributionList
{
    public interface IRecipientsRepository
    {
        Task AddAsync(Identity value);
        Task<bool> ContainsAsync(Identity value);
        Task<bool> TryRemoveAsync(Identity value);
        Task<IAsyncEnumerable<Identity>> AsEnumerableAsync();
    }
}