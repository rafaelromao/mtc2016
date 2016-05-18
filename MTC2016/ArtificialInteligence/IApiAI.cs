using System.Collections.Generic;
using System.Threading.Tasks;
using Lime.Protocol;
using MTC2016.Scheduler;

namespace MTC2016.ArtificialInteligence
{
    public interface IApiAi
    {
        Task<string> GetAnswerAsync(string question);
        Task<IEnumerable<Intent>> GetIntentsAsync();
        Task<Intent> GetIntentAsync(string intentId);
    }
}
