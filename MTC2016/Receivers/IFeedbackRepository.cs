using System.Threading.Tasks;

namespace MTC2016.Receivers
{
    public interface IFeedbackRepository
    {
        Task<bool> AddFeedbackAsync(Feedback feedback);
    }
}