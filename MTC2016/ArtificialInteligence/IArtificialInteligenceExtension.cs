using System.Collections.Generic;
using System.Threading.Tasks;
using Lime.Protocol;
using MTC2016.Scheduler;

namespace MTC2016.ArtificialInteligence
{
    public interface IArtificialInteligenceExtension
    {
        Task<string> GetAnswerAsync(string question);
        Task<IEnumerable<ScheduledMessage>> GetScheduledMessagesAsync();
        Task AddEntityAsync(string toString);
        Task<IEnumerable<Identity>> GetUsersAsync();
        Task<bool> ContainsUserAsync(Identity user);
        Task<bool> RemoveUserAsync(Identity recipient);
    }
}
