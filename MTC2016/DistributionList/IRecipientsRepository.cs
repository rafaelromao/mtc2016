using System.Collections.Generic;
using System.Threading.Tasks;
using Lime.Protocol;

namespace MTC2016.DistributionList
{
    internal interface IRecipientsRepository
    {
        Task<bool> AddUserAsync(Identity user);
        Task<IEnumerable<Identity>> GetUsersAsync();
        Task<bool> ContainsUserAsync(Identity user);
        Task<bool> RemoveUserAsync(Identity user);
    }
}