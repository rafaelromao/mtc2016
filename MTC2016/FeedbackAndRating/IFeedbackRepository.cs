using System.Threading.Tasks;

namespace MTC2016.FeedbackAndRating
{
    public interface IFeedbackRepository
    {
        Task AddAsync(Feedback feedback);
    }
}