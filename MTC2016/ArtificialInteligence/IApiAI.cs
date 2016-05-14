using System.Collections.Generic;
using System.Threading.Tasks;
using Lime.Protocol;
using MTC2016.Scheduler;

namespace MTC2016.ArtificialInteligence
{
    public interface IApiAI
    {
        Task<string> GetAnswerAsync(string question);
        Task<IEnumerable<Intent>> GetIntentsAsync();
        Task<Intent> GetIntentAsync(string intentId);
        Task<bool> AddIntentAsync(Intent intent);
        Task<bool> DeleteIntent(string intentQuestion);

        //Task<IEnumerable<Node>> GetUsersAsync();
        //Task<bool> ContainsUserAsync(Node user);
        //Task<bool> RemoveUserAsync(Node user);
        //Task<bool> AddUserAsync(Node user);
        Task<bool> AddFeedbackAsync(string feedbackId, string feedback);
    }
}
