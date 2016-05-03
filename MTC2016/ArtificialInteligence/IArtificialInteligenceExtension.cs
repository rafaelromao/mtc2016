using System.Collections.Generic;
using System.Threading.Tasks;
using MTC2016.Scheduler;

namespace MTC2016.ArtificialInteligence
{
    public interface IArtificialInteligenceExtension
    {
        Task<string> GetAnswerAsync(string question);
        Task<IEnumerable<ScheduledMessage>> GetScheduledMessagesAsync();
    }
}
