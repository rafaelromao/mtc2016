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
        Task<bool> AddUserAsync(Node user);
        Task<IEnumerable<Node>> GetUsersAsync();
        Task<bool> ContainsUserAsync(Node user);
        Task<bool> RemoveUserAsync(Node user);
    }
}
