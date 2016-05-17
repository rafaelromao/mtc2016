using System.Threading.Tasks;

namespace MTC2016.Receivers
{
    public interface IFeedbackRepository
    {
        Task AddAsync(Feedback feedback);
    }
}